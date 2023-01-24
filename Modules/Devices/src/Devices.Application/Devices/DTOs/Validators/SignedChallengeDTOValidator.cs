using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Devices.Application.Devices.DTOs.Validators;

public class SignedChallengeDTOValidator : AbstractValidator<SignedChallengeDTO>
{
    public SignedChallengeDTOValidator()
    {
        RuleFor(c => c.Signature).DetailedNotEmpty();
        RuleFor(c => c.Challenge).DetailedNotEmpty();
    }
}
