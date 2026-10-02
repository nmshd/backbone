using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Notifications.SendNotification;

public class SendNotificationCommand : IRequest
{
    public required string[] Recipients { get; init; }
    public required string Code { get; init; }
}
