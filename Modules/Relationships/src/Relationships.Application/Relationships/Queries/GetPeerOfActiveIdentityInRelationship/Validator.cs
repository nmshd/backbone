using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetPeerOfActiveIdentityInRelationship;

public class Validator : AbstractValidator<GetPeerOfActiveIdentityInRelationshipQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetPeerOfActiveIdentityInRelationshipQuery, RelationshipId>();
    }
}
