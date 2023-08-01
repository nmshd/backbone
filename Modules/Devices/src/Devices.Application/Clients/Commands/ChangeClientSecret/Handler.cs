using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
public class Handler : IRequestHandler<ChangeClientSecretCommand, ChangeClientSecretResponse>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;

    public Handler(IOAuthClientsRepository oAuthClientsRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
    }

    public async Task<ChangeClientSecretResponse> Handle(ChangeClientSecretCommand request, CancellationToken cancellationToken)
    {
        var client = await _oAuthClientsRepository.Find(request.ClientId, cancellationToken);

        var clientSecret = string.IsNullOrEmpty(request.NewSecret) ? PasswordGenerator.Generate(30) : request.NewSecret;
        client.ClientSecret = clientSecret;

        await _oAuthClientsRepository.Update(client, cancellationToken);

        return new ChangeClientSecretResponse(client.ClientId, client.DisplayName, clientSecret);
    }
}
