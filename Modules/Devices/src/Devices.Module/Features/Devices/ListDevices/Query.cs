using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.ListDevices;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListDevicesQuery")]
public class Query : IRequest<Response>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required IEnumerable<string> Ids { get; init; }
}
