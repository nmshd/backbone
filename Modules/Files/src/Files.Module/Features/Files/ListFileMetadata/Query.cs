using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.ListFileMetadata;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListFileMetadataQuery")]
public class Query : IRequest<Response>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required IEnumerable<string> Ids { get; init; }
}
