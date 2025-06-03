using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsOfIdentity;

public class ListRelationshipsOfIdentityResponse : CollectionResponseBase<Relationship>
{
    public ListRelationshipsOfIdentityResponse(IEnumerable<Relationship> items) : base(items)
    {
    }
}
