using Domain.Repositories;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.Projects.GetProjectById;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<Result<ProjectDto>>;

public sealed class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    public GetProjectByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class GetProjectByIdQueryHandler(IProjectRepository projects)
    : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.Id, cancellationToken);
        if (project is null)
            return Result.Failure<ProjectDto>(Error.NotFound(nameof(Project), request.Id));

        return Result.Success(project.ToDto());
    }
}
