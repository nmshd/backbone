using MediatR;

namespace Devices.Application.PushNotifications.Commands.SendTestNotification;

public class SendTestNotificationCommand : IRequest<Unit>
{
    public object Data { get; set; }
}
