using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.ListModifications;

public class ListModificationsQuery : IRequest<ListModificationsResponse>
{
    public long? LocalIndex { get; init; }
    public required ushort SupportedDatawalletVersion { get; init; }
    public PaginationFilter PaginationFilter { get; set; } = new(1, 250);
}
