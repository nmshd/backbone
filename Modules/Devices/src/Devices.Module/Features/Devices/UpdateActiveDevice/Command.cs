using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.UpdateActiveDevice;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("UpdateActiveDeviceCommand")]
public class Command : IRequest
{
    public required string CommunicationLanguage { get; init; }
}
