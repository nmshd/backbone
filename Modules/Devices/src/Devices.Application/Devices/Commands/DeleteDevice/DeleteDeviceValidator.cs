using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;

// ReSharper disable once UnusedMember.Global
public class DeleteDeviceValidator : AbstractValidator<DeleteDeviceCommand>
{
    public DeleteDeviceValidator()
    {
        RuleFor(c => c.DeviceId).DetailedNotEmpty();
    }
}
