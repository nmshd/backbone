using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class CreateRelationshipTemplateResponse
{
    public CreateRelationshipTemplateResponse(RelationshipTemplate relationshipTemplate)
    {
        Id = relationshipTemplate.Id;
        CreatedAt = relationshipTemplate.CreatedAt;
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
