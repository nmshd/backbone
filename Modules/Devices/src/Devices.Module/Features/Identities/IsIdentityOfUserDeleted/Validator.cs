using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Identities.IsIdentityOfUserDeleted;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(x => x.Username).ValidId<Query, Username>();
    }
}
