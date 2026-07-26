using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using MediatR;

namespace Application.Projects.Queries;

public sealed record GetAllProjectsQuery : IRequest<IReadOnlyList<ProjectDto>>;

public sealed class GetAllProjectsQueryHandler(IProjectRepository projects)
    : IRequestHandler<GetAllProjectsQuery, IReadOnlyList<ProjectDto>>
{
    public async Task<IReadOnlyList<ProjectDto>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await projects.GetAllAsync(cancellationToken);
        return items.Select(p => p.ToDto()).ToList();
    }
}
