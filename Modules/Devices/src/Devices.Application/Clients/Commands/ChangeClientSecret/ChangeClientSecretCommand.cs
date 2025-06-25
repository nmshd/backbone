using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;

public class ChangeClientSecretCommand : IRequest<ChangeClientSecretResponse>
{
    public required string ClientId { get; set; }

    public required string NewSecret { get; set; }
}
