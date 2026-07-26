using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Projects.Queries;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto>;

public sealed class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    public GetProjectByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class GetProjectByIdQueryHandler(IProjectRepository projects)
    : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        return project.ToDto();
    }
}
