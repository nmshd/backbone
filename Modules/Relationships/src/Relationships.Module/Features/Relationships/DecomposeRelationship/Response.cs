using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeRelationship;

public class DecomposeRelationshipResponse : RelationshipMetadataDTO
{
    public DecomposeRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
