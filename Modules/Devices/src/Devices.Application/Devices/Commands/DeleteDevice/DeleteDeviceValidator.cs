using Devices.Application.Devices.DTOs.Validators;
using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Devices.Application.Devices.Commands.DeleteDevice;

// ReSharper disable once UnusedMember.Global
public class DeleteDeviceValidator : AbstractValidator<DeleteDeviceCommand>
{
    public DeleteDeviceValidator()
    {
        RuleFor(c => c.DeviceId).DetailedNotEmpty();

        RuleFor(c => c.DeletionCertificate).DetailedNotEmpty();

        RuleFor(c => c.SignedChallenge)
            .Cascade(CascadeMode.Stop)
            .DetailedNotEmpty()
            .SetValidator(new SignedChallengeDTOValidator());
    }
}
