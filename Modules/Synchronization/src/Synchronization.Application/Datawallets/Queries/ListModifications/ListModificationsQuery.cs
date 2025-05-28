using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.ListModifications;

public class ListModificationsQuery : IRequest<ListModificationsResponse>
{
    // used for testing purposes only
    internal ListModificationsQuery(long? localIndex, ushort supportedDatawalletVersion) : this(new PaginationFilter(1, 250), localIndex, supportedDatawalletVersion)
    {
    }

    [JsonConstructor]
    public ListModificationsQuery(PaginationFilter paginationFilter, long? localIndex, ushort supportedDatawalletVersion)
    {
        PaginationFilter = paginationFilter;
        LocalIndex = localIndex;
        SupportedDatawalletVersion = supportedDatawalletVersion;
    }

    public long? LocalIndex { get; set; }
    public ushort SupportedDatawalletVersion { get; set; }
    public PaginationFilter PaginationFilter { get; set; }
}
