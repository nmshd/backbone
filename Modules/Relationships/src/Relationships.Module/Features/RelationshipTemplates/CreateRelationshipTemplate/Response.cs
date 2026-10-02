using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.CreateRelationshipTemplate;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateRelationshipTemplateResponse")]
public class Response
{
    public Response(RelationshipTemplate relationshipTemplate)
    {
        Id = relationshipTemplate.Id;
        CreatedAt = relationshipTemplate.CreatedAt;
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
