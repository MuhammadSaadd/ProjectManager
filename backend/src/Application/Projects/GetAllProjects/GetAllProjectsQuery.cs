using Domain.Repositories;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Primitives;
using MediatR;

namespace Application.Projects.GetAllProjects;

public sealed record GetAllProjectsQuery : IRequest<Result<IReadOnlyList<ProjectDto>>>;

public sealed class GetAllProjectsQueryHandler(IProjectRepository projects)
    : IRequestHandler<GetAllProjectsQuery, Result<IReadOnlyList<ProjectDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectDto>>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await projects.GetAllAsync(cancellationToken);
        return Result.Success(items.Select(p => p.ToDto()).ToList() as IReadOnlyList<ProjectDto>);
    }
}
