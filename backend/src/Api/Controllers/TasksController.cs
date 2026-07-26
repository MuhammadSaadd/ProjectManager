using Application.Common.Models;
using Application.Tasks.Commands;
using Application.Tasks.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<TaskItemDto>>> GetByStatus(
        [FromQuery] TaskStatus? status,
        CancellationToken cancellationToken)
    {
        if (status is null)
        {
            return BadRequest(new { detail = "Query parameter 'status' is required." });
        }

        var result = await mediator.Send(new GetTasksByStatusQuery(status.Value), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTaskByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateTaskCommand(
                request.ProjectId,
                request.Title,
                request.Description,
                request.DueDate,
                request.Status),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> Update(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateTaskCommand(
                id,
                request.Title,
                request.Description,
                request.DueDate,
                request.Status),
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> ChangeStatus(
        Guid id,
        [FromBody] ChangeTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ChangeTaskStatusCommand(id, request.Status),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTaskCommand(id), cancellationToken);
        return NoContent();
    }
}

public sealed record CreateTaskRequest(
    Guid ProjectId,
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskStatus Status = TaskStatus.ToDo);

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskStatus Status);

public sealed record ChangeTaskStatusRequest(TaskStatus Status);
