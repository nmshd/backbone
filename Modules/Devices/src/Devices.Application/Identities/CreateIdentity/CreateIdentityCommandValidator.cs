using Devices.Application.Devices.DTOs.Validators;
using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Devices.Application.Identities.CreateIdentity;

// ReSharper disable once UnusedMember.Global
public class CreateIdentityCommandValidator : AbstractValidator<CreateIdentityCommand>
{
    public CreateIdentityCommandValidator()
    {
        RuleFor(c => c.IdentityPublicKey).DetailedNotEmpty();
        RuleFor(c => c.DevicePassword).DetailedNotEmpty();
        RuleFor(c => c.SignedChallenge).DetailedNotEmpty().SetValidator(new SignedChallengeDTOValidator());
    }
}
