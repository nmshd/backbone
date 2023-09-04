using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Domain.Errors;
using MediatR;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Clients.Commands.CreateClients;

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
        if (!string.IsNullOrEmpty(request.ClientId))
        {
            var clientExists = await _oAuthClientsRepository.Exists(request.ClientId, cancellationToken);
            if (clientExists)
                throw new ApplicationException(ApplicationErrors.Devices.ClientIdAlreadyExists());
        }

        if (!string.IsNullOrEmpty(request.TierId))
        {
            var tierIdResult = TierId.Create(request.TierId);
            if (tierIdResult.IsFailure)
                throw new ApplicationException(ApplicationErrors.Devices.InvalidTierId());

            _ = await _tiersRepository.FindById(tierIdResult.Value, cancellationToken) ?? throw new ApplicationException(GenericApplicationErrors.NotFound(nameof(Tier)));
        }
        else
        {
            var basicTier = await _tiersRepository.GetBasicTierAsync(cancellationToken);
            request.TierId = basicTier.Id.Value;
        }

        var clientSecret = string.IsNullOrEmpty(request.ClientSecret) ? PasswordGenerator.Generate(30) : request.ClientSecret;
        var clientId = string.IsNullOrEmpty(request.ClientId) ? ClientIdGenerator.Generate() : request.ClientId;
        var displayName = string.IsNullOrEmpty(request.DisplayName) ? clientId : request.DisplayName;
        var tierId = TierId.Create(request.TierId).Value;

        await _oAuthClientsRepository.Add(clientId, displayName, clientSecret, tierId, cancellationToken);

        return new CreateClientResponse(clientId, displayName, clientSecret, tierId);
    }
}
