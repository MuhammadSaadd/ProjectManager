using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Projects.Commands;

public sealed record UpdateProjectCommand(Guid Id, string Name, string? Description) : IRequest<ProjectDto>;

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
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        project.UpdateDetails(request.Name, request.Description);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return project.ToDto();
    }
}
