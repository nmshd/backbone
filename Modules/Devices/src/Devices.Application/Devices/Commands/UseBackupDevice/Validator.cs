using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Commands.UseBackupDevice;

public class Validator : AbstractValidator<UseBackupDeviceCommand>
{
    public Validator()
    {
        RuleFor(command => command.DeviceId).ValidId<UseBackupDeviceCommand, DeviceId>();
    }
}
