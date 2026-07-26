using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using FluentValidation;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Tasks.Queries;

public sealed record GetTasksByStatusQuery(TaskStatus Status) : IRequest<IReadOnlyList<TaskItemDto>>;

public sealed class GetTasksByStatusQueryValidator : AbstractValidator<GetTasksByStatusQuery>
{
    public GetTasksByStatusQueryValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class GetTasksByStatusQueryHandler(ITaskRepository tasks)
    : IRequestHandler<GetTasksByStatusQuery, IReadOnlyList<TaskItemDto>>
{
    public async Task<IReadOnlyList<TaskItemDto>> Handle(
        GetTasksByStatusQuery request,
        CancellationToken cancellationToken)
    {
        var items = await tasks.GetByStatusAsync(request.Status, cancellationToken);
        return items.Select(t => t.ToDto()).ToList();
    }
}
