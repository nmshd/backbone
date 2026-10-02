using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.UpdateActiveDevice;

public class Command : IRequest
{
    public required string CommunicationLanguage { get; init; }
}
