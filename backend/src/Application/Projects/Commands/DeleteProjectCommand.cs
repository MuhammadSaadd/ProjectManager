using Application.Common.Interfaces;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Projects.Commands;

public sealed record DeleteProjectCommand(Guid Id) : IRequest;

public sealed class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class DeleteProjectCommandHandler(
    IProjectRepository projects,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        projects.Remove(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
