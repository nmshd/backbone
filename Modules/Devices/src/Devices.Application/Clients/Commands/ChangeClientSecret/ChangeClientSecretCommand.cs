using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
public class ChangeClientSecretCommand : IRequest<ChangeClientSecretResponse>
{
    public ChangeClientSecretCommand(string clientId, string newSecret)
    {
        ClientId = clientId;
        NewSecret = newSecret;
    }

    public string ClientId { get; set; }

    public string NewSecret { get; set; }
}
