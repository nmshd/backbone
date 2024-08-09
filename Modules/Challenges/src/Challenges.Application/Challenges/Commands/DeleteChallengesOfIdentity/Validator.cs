using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengesOfIdentity;

public class Validator : AbstractValidator<DeleteChallengesOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<DeleteChallengesOfIdentityCommand, IdentityAddress>();
    }
}
