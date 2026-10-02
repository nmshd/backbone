using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Module.Features.Clients.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.ChangeClientSecret;

public class Handler : IRequestHandler<ChangeClientSecretCommand, ChangeClientSecretResponse>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;

    public Handler(IOAuthClientsRepository oAuthClientsRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
    }

    public async Task<ChangeClientSecretResponse> Handle(ChangeClientSecretCommand request, CancellationToken cancellationToken)
    {
        var client = await _oAuthClientsRepository.Get(request.ClientId, cancellationToken, track: true) ?? throw new NotFoundException(nameof(OAuthClient));

        var clientSecret = string.IsNullOrEmpty(request.NewSecret) ? PasswordGenerator.Generate(30) : request.NewSecret;

        await _oAuthClientsRepository.ChangeClientSecret(client, clientSecret, cancellationToken);

        return new ChangeClientSecretResponse(client, clientSecret);
    }
}
