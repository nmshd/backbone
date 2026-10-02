using MediatR;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.SendTestNotification;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("SendTestNotificationCommand")]
public class Command : IRequest<Unit>
{
    public required object Data { get; init; }
}
