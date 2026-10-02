using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.UpdateClient;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("UpdateClientCommand")]
public class Command : IRequest<Response>
{
    public required string ClientId { get; init; }

    public required string DefaultTier { get; init; }

    public int? MaxIdentities { get; init; }
}
