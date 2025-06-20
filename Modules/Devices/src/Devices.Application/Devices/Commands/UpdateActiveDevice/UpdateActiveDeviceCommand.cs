using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.UpdateActiveDevice;

public class UpdateActiveDeviceCommand : IRequest
{
    public required string CommunicationLanguage { get; init; }
}
