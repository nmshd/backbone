using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Devices.Application.PushNotifications.Commands.SendTestNotification;

public class Validator : AbstractValidator<SendTestNotificationCommand>
{
    public Validator()
    {
        RuleFor(r => r.Data).DetailedNotNull();
    }
}
