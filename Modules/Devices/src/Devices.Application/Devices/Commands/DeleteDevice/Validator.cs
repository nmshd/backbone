using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<DeleteDeviceCommand>
{
    public Validator()
    {
        RuleFor(c => c.DeviceId).ValidId<DeleteDeviceCommand, DeviceId>();
    }
}
