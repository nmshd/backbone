using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationshipReactivation;

public class RevokeRelationshipReactivationResponse : RelationshipMetadataDTO
{
    public RevokeRelationshipReactivationResponse(Relationship relationship) : base(relationship) { }
}
