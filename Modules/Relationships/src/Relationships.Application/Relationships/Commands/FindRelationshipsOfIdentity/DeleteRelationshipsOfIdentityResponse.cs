using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;

public class FindRelationshipsOfIdentityResponse : CollectionResponseBase<Relationship>
{
    public FindRelationshipsOfIdentityResponse(IEnumerable<Relationship> items) : base(items)
    {
    }
}
