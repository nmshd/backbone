using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.ListTiers;

public class ListTiersQuery : IRequest<ListTiersResponse>
{
    public required PaginationFilter PaginationFilter { get; init; }
}
