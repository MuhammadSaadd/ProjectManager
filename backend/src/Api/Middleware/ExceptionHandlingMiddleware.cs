using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail, errors) = exception switch
        {
            FluentValidation.ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                "One or more validation errors occurred.",
                validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()) as IDictionary<string, string[]>),

            ValidationException domainValidation => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                domainValidation.Message,
                null),

            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                notFound.Message,
                null),

            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                "An unexpected error occurred. Please try again later.",
                null)
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            logger.LogWarning(exception, "Handled exception: {Title}", title);
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errors is not null)
        {
            problem.Extensions["errors"] = errors;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}
