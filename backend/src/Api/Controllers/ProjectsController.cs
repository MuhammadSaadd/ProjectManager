using Application.Common.Models;
using Application.Projects.Commands;
using Application.Projects.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllProjectsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProjectByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> Create(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateProjectCommand(request.Name, request.Description),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateProjectCommand(id, request.Name, request.Description),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProjectCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpGet("{projectId:guid}/tasks")]
    [ProducesResponseType(typeof(IReadOnlyList<TaskItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TaskItemDto>>> GetTasks(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new Application.Tasks.Queries.GetTasksByProjectQuery(projectId),
            cancellationToken);

        return Ok(result);
    }
}

public sealed record CreateProjectRequest(string Name, string? Description);
public sealed record UpdateProjectRequest(string Name, string? Description);
