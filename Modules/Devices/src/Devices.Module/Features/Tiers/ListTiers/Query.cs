using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.ListTiers;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListTiersQuery")]
public class Query : IRequest<Response>
{
    public required PaginationFilter PaginationFilter { get; init; }
}
