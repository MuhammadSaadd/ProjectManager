using Domain.Repositories;
using Domain.Entities;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.Projects.DeleteProject;

public sealed record DeleteProjectCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class DeleteProjectCommandHandler(
    IProjectRepository projects,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProjectCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.Id, cancellationToken);
        if (project is null)
            return Result.Failure(Error.NotFound(nameof(Project), request.Id));

        projects.Remove(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
