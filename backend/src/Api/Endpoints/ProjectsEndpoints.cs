using Application.Common.Models;
using Application.Projects.CreateProject;
using Application.Projects.DeleteProject;
using Application.Projects.GetAllProjects;
using Application.Projects.GetProjectById;
using Application.Projects.UpdateProject;
using Application.Tasks.GetTasksByProject;
using Domain.Primitives;
using MediatR;

namespace Api.Endpoints;

public static class ProjectsEndpoints
{
    public static void MapProjectsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projects");

        group.MapGet("/", GetAllProjects);
        group.MapGet("/{id:guid}", GetProjectById);
        group.MapPost("/", CreateProject);
        group.MapPut("/{id:guid}", UpdateProject);
        group.MapDelete("/{id:guid}", DeleteProject);
        group.MapGet("/{projectId:guid}/tasks", GetProjectTasks);
    }

    private static async Task<IResult> GetAllProjects(IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllProjectsQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : ErrorToResult(result.Error!);
    }

    private static async Task<IResult> GetProjectById(Guid id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProjectByIdQuery(id), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : ErrorToResult(result.Error!);
    }

    private static async Task<IResult> CreateProject(CreateProjectRequest request, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateProjectCommand(request.Name, request.Description), ct);

        if (result.IsFailure)
            return ErrorToResult(result.Error!);

        return Results.Created($"/api/projects/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> UpdateProject(Guid id, UpdateProjectRequest request, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateProjectCommand(id, request.Name, request.Description), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : ErrorToResult(result.Error!);
    }

    private static async Task<IResult> DeleteProject(Guid id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteProjectCommand(id), ct);
        return result.IsSuccess
            ? Results.NoContent()
            : ErrorToResult(result.Error!);
    }

    private static async Task<IResult> GetProjectTasks(Guid projectId, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetTasksByProjectQuery(projectId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
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

    private sealed record CreateProjectRequest(string Name, string? Description);
    private sealed record UpdateProjectRequest(string Name, string? Description);
}
