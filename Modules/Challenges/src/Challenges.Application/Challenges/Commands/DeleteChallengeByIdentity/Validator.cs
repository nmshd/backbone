using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengeByIdentity;
public class Validator : AbstractValidator<DeleteChallengeByIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
