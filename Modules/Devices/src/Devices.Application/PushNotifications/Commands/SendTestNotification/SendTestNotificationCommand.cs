using MediatR;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.SendTestNotification;

public class SendTestNotificationCommand : IRequest<Unit>
{
    public required object Data { get; init; }
}
