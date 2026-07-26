using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Projects.Commands;

public sealed record CreateProjectCommand(string Name, string? Description) : IRequest<ProjectDto>;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description is not null);
    }
}

public sealed class CreateProjectCommandHandler(
    IProjectRepository projects,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project(request.Name, request.Description);
        await projects.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return project.ToDto();
    }
}
