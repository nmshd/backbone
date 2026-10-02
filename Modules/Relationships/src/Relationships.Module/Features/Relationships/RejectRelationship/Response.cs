using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RejectRelationshipResponse")]
public class Response : RelationshipMetadataDTO
{
    public Response(Relationship relationship) : base(relationship)
    {
    }
}
