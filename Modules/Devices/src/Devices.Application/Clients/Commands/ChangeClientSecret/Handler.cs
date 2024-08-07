using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
public class Handler : IRequestHandler<ChangeClientSecretCommand, ChangeClientSecretResponse>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IOAuthClientsRepository oAuthClientsRepository, IIdentitiesRepository identitiesRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<ChangeClientSecretResponse> Handle(ChangeClientSecretCommand request, CancellationToken cancellationToken)
    {
        var client = await _oAuthClientsRepository.Find(request.ClientId, cancellationToken, track: true) ?? throw new NotFoundException(nameof(OAuthClient));

        var clientSecret = string.IsNullOrEmpty(request.NewSecret) ? PasswordGenerator.Generate(30) : request.NewSecret;

        await _oAuthClientsRepository.ChangeClientSecret(client, clientSecret, cancellationToken);

        var numberOfIdentities = await _identitiesRepository.CountByClientId(request.ClientId, cancellationToken);

        return new ChangeClientSecretResponse(client, clientSecret, numberOfIdentities);
    }
}
