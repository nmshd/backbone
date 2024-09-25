using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;

public class GetRelationshipTemplateQuery : IRequest<RelationshipTemplateDTO>
{
    public required string Id { get; set; }
    public byte[] Password { get; set; } = Array.Empty<byte>();
}
