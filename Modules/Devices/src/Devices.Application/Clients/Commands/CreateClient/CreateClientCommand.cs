using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.CreateClient;

public class CreateClientCommand : IRequest<CreateClientResponse>
{
    public string? ClientId { get; init; }

    public string? DisplayName { get; init; }

    public string? ClientSecret { get; init; }

    public required string DefaultTier { get; init; }

    public int? MaxIdentities { get; init; }
}
