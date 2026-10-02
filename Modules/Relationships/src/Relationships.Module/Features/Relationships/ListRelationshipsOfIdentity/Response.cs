using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationshipsOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListRelationshipsOfIdentityResponse")]
public class Response : CollectionResponseBase<Relationship>
{
    public Response(IEnumerable<Relationship> items) : base(items)
    {
    }
}
