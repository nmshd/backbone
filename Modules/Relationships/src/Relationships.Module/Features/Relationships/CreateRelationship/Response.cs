using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CreateRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateRelationshipResponse")]
public class Response : RelationshipMetadataDTO
{
    public Response(Relationship relationship) : base(relationship)
    {
    }
}
