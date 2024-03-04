using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetModifications;

public class GetModificationsQuery : IRequest<GetModificationsResponse>
{
    // used for testing purposes only
    internal GetModificationsQuery(long? localIndex, ushort supportedDatawalletVersion) : this(new PaginationFilter(1, 250), localIndex, supportedDatawalletVersion) { }

    [JsonConstructor]
    public GetModificationsQuery(PaginationFilter paginationFilter, long? localIndex, ushort supportedDatawalletVersion)
    {
        PaginationFilter = paginationFilter;
        LocalIndex = localIndex;
        SupportedDatawalletVersion = supportedDatawalletVersion;
    }

    public long? LocalIndex { get; set; }
    public ushort SupportedDatawalletVersion { get; set; }
    public PaginationFilter PaginationFilter { get; set; }
}
