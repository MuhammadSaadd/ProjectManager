using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Tasks.Queries;

public sealed record GetTaskByIdQuery(Guid Id) : IRequest<TaskItemDto>;

public sealed class GetTaskByIdQueryValidator : AbstractValidator<GetTaskByIdQuery>
{
    public GetTaskByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class GetTaskByIdQueryHandler(ITaskRepository tasks)
    : IRequestHandler<GetTaskByIdQuery, TaskItemDto>
{
    public async Task<TaskItemDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await tasks.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.Id);

        return task.ToDto();
    }
}
