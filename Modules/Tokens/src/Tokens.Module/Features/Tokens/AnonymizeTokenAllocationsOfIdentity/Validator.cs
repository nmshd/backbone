using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokenAllocationsOfIdentity;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(q => q.IdentityAddress)
            .ValidId<Command, IdentityAddress>();
    }
}
