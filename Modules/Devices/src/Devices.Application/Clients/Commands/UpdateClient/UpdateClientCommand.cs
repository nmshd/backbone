using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;

public class UpdateClientCommand : IRequest<UpdateClientResponse>
{
    public required string ClientId { get; init; }

    public required string DefaultTier { get; init; }

    public int? MaxIdentities { get; init; }
}
