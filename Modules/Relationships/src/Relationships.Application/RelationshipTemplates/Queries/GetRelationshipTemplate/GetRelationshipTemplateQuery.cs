using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;

public class GetRelationshipTemplateQuery : IRequest<RelationshipTemplateDTO>
{
    public required string Id { get; init; }
    public byte[]? Password { get; init; }
}
