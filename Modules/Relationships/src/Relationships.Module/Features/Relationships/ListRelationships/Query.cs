using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationships;

public class ListRelationshipsQuery : IRequest<ListRelationshipsResponse>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required List<string> Ids { get; set; }
}
