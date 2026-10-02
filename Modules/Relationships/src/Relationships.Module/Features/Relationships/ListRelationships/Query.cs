using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationships;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListRelationshipsQuery")]
public class Query : IRequest<Response>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required List<string> Ids { get; set; }
}
