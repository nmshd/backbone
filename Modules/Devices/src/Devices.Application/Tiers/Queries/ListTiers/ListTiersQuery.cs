using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;

public class ListTiersQuery : IRequest<ListTiersResponse>
{
    public required PaginationFilter PaginationFilter { get; init; }
}
