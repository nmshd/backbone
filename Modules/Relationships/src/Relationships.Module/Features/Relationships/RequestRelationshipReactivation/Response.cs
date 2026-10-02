using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RequestRelationshipReactivation;

public class RequestRelationshipReactivationResponse : RelationshipMetadataDTO
{
    public RequestRelationshipReactivationResponse(Relationship relationship) : base(relationship)
    {
    }
}
