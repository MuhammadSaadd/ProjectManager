using Domain.Repositories;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.Projects.CreateProject;

public sealed record CreateProjectCommand(string Name, string? Description) : IRequest<Result<ProjectDto>>;

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
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var projectResult = Project.Create(request.Name, request.Description);
        if (projectResult.IsFailure)
            return Result.Failure<ProjectDto>(projectResult.Error!);

        var project = projectResult.Value;
        await projects.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(project.ToDto());
    }
}
