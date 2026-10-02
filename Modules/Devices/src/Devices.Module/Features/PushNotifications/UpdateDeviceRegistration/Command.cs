using MediatR;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.UpdateDeviceRegistration;

public class Command : IRequest<Response>
{
    public required string Platform { get; init; }
    public required string Handle { get; init; }
    public required string AppId { get; init; }
    public string? Environment { get; init; }
}
