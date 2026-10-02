using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationshipReactivation;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RevokeRelationshipReactivationResponse")]
public class Response : RelationshipMetadataDTO
{
    public Response(Relationship relationship) : base(relationship) { }
}
