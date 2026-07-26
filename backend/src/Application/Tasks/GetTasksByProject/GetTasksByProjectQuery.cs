using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.Tasks.GetTasksByProject;

public sealed record GetTasksByProjectQuery(Guid ProjectId) : IRequest<Result<IReadOnlyList<TaskItemDto>>>;

public sealed class GetTasksByProjectQueryValidator : AbstractValidator<GetTasksByProjectQuery>
{
    public GetTasksByProjectQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

public sealed class GetTasksByProjectQueryHandler(
    IProjectRepository projects,
    ITaskRepository tasks) : IRequestHandler<GetTasksByProjectQuery, Result<IReadOnlyList<TaskItemDto>>>
{
    public async Task<Result<IReadOnlyList<TaskItemDto>>> Handle(
        GetTasksByProjectQuery request,
        CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure<IReadOnlyList<TaskItemDto>>(Error.NotFound(nameof(Project), request.ProjectId));

        var items = await tasks.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        return Result.Success(items.Select(t => t.ToDto()).ToList() as IReadOnlyList<TaskItemDto>);
    }
}
