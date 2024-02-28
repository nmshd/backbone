using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;

public class FindRelationshipsOfIdentityResponse : CollectionResponseBase<Relationship>
{
    public FindRelationshipsOfIdentityResponse(IEnumerable<Relationship> items) : base(items)
    {
    }
}
