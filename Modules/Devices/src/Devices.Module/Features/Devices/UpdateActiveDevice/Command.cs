using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.UpdateActiveDevice;

public class UpdateActiveDeviceCommand : IRequest
{
    public required string CommunicationLanguage { get; init; }
}
