using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.AnonymizeTokenAllocationsOfIdentity;

public class Validator : AbstractValidator<AnonymizeTokenAllocationsOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(q => q.IdentityAddress)
            .ValidId<AnonymizeTokenAllocationsOfIdentityCommand, IdentityAddress>();
    }
}
