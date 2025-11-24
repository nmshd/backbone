using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokenAllocationsOfIdentity;

public class Validator : AbstractValidator<DeleteTokenAllocationsOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(q => q.IdentityAddress)
            .ValidId<DeleteTokenAllocationsOfIdentityCommand, IdentityAddress>();
    }
}
