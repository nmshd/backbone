using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.SendTestNotification;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(r => r.Data).DetailedNotNull();
    }
}
