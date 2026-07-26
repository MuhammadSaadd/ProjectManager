using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Tasks.Queries;

public sealed record GetTasksByProjectQuery(Guid ProjectId) : IRequest<IReadOnlyList<TaskItemDto>>;

public sealed class GetTasksByProjectQueryValidator : AbstractValidator<GetTasksByProjectQuery>
{
    public GetTasksByProjectQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

public sealed class GetTasksByProjectQueryHandler(
    IProjectRepository projects,
    ITaskRepository tasks) : IRequestHandler<GetTasksByProjectQuery, IReadOnlyList<TaskItemDto>>
{
    public async Task<IReadOnlyList<TaskItemDto>> Handle(
        GetTasksByProjectQuery request,
        CancellationToken cancellationToken)
    {
        _ = await projects.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        var items = await tasks.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        return items.Select(t => t.ToDto()).ToList();
    }
}
