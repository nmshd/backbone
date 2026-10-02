using MediatR;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.SendTestNotification;

public class Command : IRequest<Unit>
{
    public required object Data { get; init; }
}
