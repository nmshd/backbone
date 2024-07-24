using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;

public class Validator : AbstractValidator<DeleteIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<DeleteIdentityCommand, IdentityAddress>();
    }
}
