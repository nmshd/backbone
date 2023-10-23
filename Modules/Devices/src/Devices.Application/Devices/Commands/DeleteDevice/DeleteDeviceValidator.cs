using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Devices.Application.Devices.DTOs.Validators;
using FluentValidation;

namespace Backbone.Devices.Application.Devices.Commands.DeleteDevice;

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
