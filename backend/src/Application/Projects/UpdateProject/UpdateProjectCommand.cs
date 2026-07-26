using Domain.Repositories;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.Projects.UpdateProject;

public sealed record UpdateProjectCommand(Guid Id, string Name, string? Description) : IRequest<Result<ProjectDto>>;

public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description is not null);
    }
}

public sealed class UpdateProjectCommandHandler(
    IProjectRepository projects,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.Id, cancellationToken);
        if (project is null)
            return Result.Failure<ProjectDto>(Error.NotFound(nameof(Project), request.Id));

        var updateResult = project.UpdateDetails(request.Name, request.Description);
        if (updateResult.IsFailure)
            return Result.Failure<ProjectDto>(updateResult.Error!);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(project.ToDto());
    }
}
