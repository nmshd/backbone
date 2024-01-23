using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Devices.Application.Devices.DTOs.Validators;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;

public class RegisterDeviceCommandValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceCommandValidator()
    {
        RuleFor(c => c.DevicePassword).DetailedNotEmpty();
        RuleFor(c => c.SignedChallenge).DetailedNotEmpty().SetValidator(new SignedChallengeDTOValidator());
    }
}
