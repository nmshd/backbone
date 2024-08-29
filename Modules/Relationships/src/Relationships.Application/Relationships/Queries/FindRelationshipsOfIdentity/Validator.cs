using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.FindRelationshipsOfIdentity;

public class Validator : AbstractValidator<FindRelationshipsOfIdentityQuery>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<FindRelationshipsOfIdentityQuery, IdentityAddress>();
    }
}
