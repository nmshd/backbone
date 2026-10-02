using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;

public class Query : IRequest<Response>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required List<string> Ids { get; init; }
}
