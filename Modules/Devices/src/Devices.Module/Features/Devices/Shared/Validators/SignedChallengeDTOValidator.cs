using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Devices.Shared.Validators;

public class SignedChallengeDTOValidator : AbstractValidator<SignedChallengeDTO>
{
    public SignedChallengeDTOValidator()
    {
        RuleFor(c => c.Signature).DetailedNotEmpty();
        RuleFor(c => c.Challenge).DetailedNotEmpty();
    }
}
