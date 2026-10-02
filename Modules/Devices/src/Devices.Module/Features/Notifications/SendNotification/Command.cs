using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Notifications.SendNotification;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("SendNotificationCommand")]
public class Command : IRequest
{
    public required string[] Recipients { get; init; }
    public required string Code { get; init; }
}
