using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsOfIdentity;

public class Validator : AbstractValidator<ListRelationshipsOfIdentityQuery>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<ListRelationshipsOfIdentityQuery, IdentityAddress>();
    }
}
