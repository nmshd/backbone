using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.ListModifications;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListModificationsQuery")]
public class Query : IRequest<Response>
{
    public long? LocalIndex { get; init; }
    public required ushort SupportedDatawalletVersion { get; init; }
    public PaginationFilter PaginationFilter { get; set; } = new(1, 250);
}
