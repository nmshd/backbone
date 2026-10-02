using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.ListFileMetadata;

public class ListFileMetadataQuery : IRequest<ListFileMetadataResponse>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required IEnumerable<string> Ids { get; init; }
}
