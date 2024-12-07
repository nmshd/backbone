using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.AnonymizeTokensForIdentity;

public class Validator : AbstractValidator<AnonymizeTokensForIdentityCommand>
{
    public Validator()
    {
        RuleFor(c => c.IdentityAddress)
            .ValidId<AnonymizeTokensForIdentityCommand, IdentityAddress>();
    }
}
