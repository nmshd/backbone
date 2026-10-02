using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Abstractions;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;

public class ListRelationshipTemplatesQuery : IRequest<ListRelationshipTemplatesResponse>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required List<ListRelationshipTemplatesQueryItem> QueryItems { get; init; }
}

