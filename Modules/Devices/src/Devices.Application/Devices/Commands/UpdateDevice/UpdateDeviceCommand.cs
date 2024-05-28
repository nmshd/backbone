using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.UpdateDevice;

public class UpdateDeviceCommand : IRequest
{
    public required string CommunicationLanguage { get; set; }
}
