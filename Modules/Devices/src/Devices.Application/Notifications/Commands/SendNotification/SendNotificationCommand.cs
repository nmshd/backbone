using MediatR;

namespace Backbone.Modules.Devices.Application.Notifications.Commands.SendNotification;

public class SendNotificationCommand : IRequest
{
    public required string[] Recipients { get; init; }
    public required string Code { get; init; }
}
