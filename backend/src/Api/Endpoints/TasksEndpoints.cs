using Application.Common.Models;
using Application.Tasks.ChangeTaskStatus;
using Application.Tasks.CreateTask;
using Application.Tasks.DeleteTask;
using Application.Tasks.GetTaskById;
using Application.Tasks.GetTasksByStatus;
using Application.Tasks.UpdateTask;
using Domain.Primitives;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Api.Endpoints;

public static class TasksEndpoints
{
    public static void MapTasksEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tasks");

        group.MapGet("/", GetByStatus);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/", CreateTask);
        group.MapPut("/{id:guid}", UpdateTask);
        group.MapPatch("/{id:guid}/status", ChangeStatus);
        group.MapDelete("/{id:guid}", DeleteTask);
    }

    private static async Task<IResult> GetByStatus(TaskStatus? status, IMediator mediator, CancellationToken ct)
    {
        if (status is null)
            return Results.BadRequest(new { detail = "Query parameter 'status' is required." });

        var result = await mediator.Send(new GetTasksByStatusQuery(status.Value), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : ErrorToResult(result.Error!);
    }

    private static async Task<IResult> GetById(Guid id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetTaskByIdQuery(id), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : ErrorToResult(result.Error!);
    }

    private static async Task<IResult> CreateTask(CreateTaskRequest request, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateTaskCommand(request.ProjectId, request.Title, request.Description, request.DueDate, request.Status), ct);

        if (result.IsFailure)
            return ErrorToResult(result.Error!);

        return Results.Created($"/api/tasks/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> UpdateTask(Guid id, UpdateTaskRequest request, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateTaskCommand(id, request.Title, request.Description, request.DueDate, request.Status), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : ErrorToResult(result.Error!);
    }

    private static async Task<IResult> ChangeStatus(Guid id, ChangeTaskStatusRequest request, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new ChangeTaskStatusCommand(id, request.Status), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : ErrorToResult(result.Error!);
    }

    private static async Task<IResult> DeleteTask(Guid id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteTaskCommand(id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : ErrorToResult(result.Error!);
    }

    private static IResult ErrorToResult(Error error)
    {
        return error.Code switch
        {
            "NotFound" => Results.NotFound(new { title = "Resource not found", detail = error.Description }),
            "Validation" => error.ValidationErrors is not null
                ? Results.BadRequest(new { title = "Validation failed", detail = error.Description, errors = error.ValidationErrors })
                : Results.BadRequest(new { title = "Validation failed", detail = error.Description }),
            _ => Results.Problem(detail: error.Description, title: "An unexpected error occurred", statusCode: 500)
        };
    }

    private sealed record CreateTaskRequest(
        Guid ProjectId,
        string Title,
        string? Description,
        DateTime? DueDate,
        TaskStatus Status = TaskStatus.ToDo);

    private sealed record UpdateTaskRequest(
        string Title,
        string? Description,
        DateTime? DueDate,
        TaskStatus Status);

    private sealed record ChangeTaskStatusRequest(TaskStatus Status);
}
