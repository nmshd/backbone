using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Queries.IsIdentityOfUserDeleted;

public class Validator : AbstractValidator<IsIdentityOfUserDeletedQuery>
{
    public Validator()
    {
        RuleFor(x => x.Username).ValidId<IsIdentityOfUserDeletedQuery, Username>();
    }
}
