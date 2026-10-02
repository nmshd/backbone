using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.ListDevices;

public class Query : IRequest<Response>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required IEnumerable<string> Ids { get; init; }
}
