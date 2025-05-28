using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Tooling;
using MediatR;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Clients.Commands.CreateClient;

public class Handler : IRequestHandler<CreateClientCommand, CreateClientResponse>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly ITiersRepository _tiersRepository;

    public Handler(IOAuthClientsRepository oAuthClientsRepository, ITiersRepository tiersRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
        _tiersRepository = tiersRepository;
    }

    public async Task<CreateClientResponse> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        await EnsureClientIdDoesNotExist(request, cancellationToken);

        var clientSecret = string.IsNullOrEmpty(request.ClientSecret) ? PasswordGenerator.Generate(30) : request.ClientSecret;
        var clientId = string.IsNullOrEmpty(request.ClientId) ? ClientIdGenerator.Generate() : request.ClientId;
        var displayName = string.IsNullOrEmpty(request.DisplayName) ? clientId : request.DisplayName;
        var defaultTierId = await GetTierId(request.DefaultTier, cancellationToken);

        var client = new OAuthClient(clientId, displayName, defaultTierId, SystemTime.UtcNow, request.MaxIdentities);

        await _oAuthClientsRepository.Add(client, clientSecret, cancellationToken);

        return new CreateClientResponse(client, clientSecret);
    }

    private async Task EnsureClientIdDoesNotExist(CreateClientCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.ClientId))
        {
            var clientExists = await _oAuthClientsRepository.Exists(request.ClientId, cancellationToken);
            if (clientExists)
                throw new ApplicationException(ApplicationErrors.Devices.ClientIdAlreadyExists());
        }
    }

    private async Task<TierId> GetTierId(string defaultTier, CancellationToken cancellationToken)
    {
        var tierNameResult = TierName.Create(defaultTier);
        if (tierNameResult.IsSuccess)
        {
            var tier = await _tiersRepository.Get(tierNameResult.Value, cancellationToken);

            if (tier != null)
                return tier.Id;
        }

        var tierIdResult = TierId.Create(defaultTier);
        if (tierIdResult.IsFailure)
            throw new ApplicationException(GenericApplicationErrors.NotFound(nameof(Tier)));

        var tierExists = await _tiersRepository.ExistsWithId(tierIdResult.Value, cancellationToken);
        if (!tierExists)
            throw new ApplicationException(GenericApplicationErrors.NotFound(nameof(Tier)));

        return tierIdResult.Value;
    }
}
