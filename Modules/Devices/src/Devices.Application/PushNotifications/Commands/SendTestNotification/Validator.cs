using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Devices.Application.PushNotifications.Commands.SendTestNotification;

public class Validator : AbstractValidator<SendTestNotificationCommand>
{
    public Validator()
    {
        RuleFor(r => r.Data).DetailedNotNull();
    }
}
