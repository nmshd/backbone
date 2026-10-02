using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.ChangeClientSecret;

public class ChangeClientSecretCommand : IRequest<ChangeClientSecretResponse>
{
    public required string ClientId { get; set; }

    public required string NewSecret { get; set; }
}
