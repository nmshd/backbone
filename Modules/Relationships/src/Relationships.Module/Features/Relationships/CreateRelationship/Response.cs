using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CreateRelationship;

public class CreateRelationshipResponse : RelationshipMetadataDTO
{
    public CreateRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
