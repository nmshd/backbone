using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Devices.DeleteDevice;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.DeviceId).ValidId<Command, DeviceId>();
    }
}
