using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Devices.Application.Devices.DTOs.Validators;
using FluentValidation;

namespace Backbone.Devices.Application.Identities.Commands.CreateIdentity;

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
