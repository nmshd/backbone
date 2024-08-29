using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;

public class Validator : AbstractValidator<DeleteTokensOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<DeleteTokensOfIdentityCommand, IdentityAddress>();
    }
}
