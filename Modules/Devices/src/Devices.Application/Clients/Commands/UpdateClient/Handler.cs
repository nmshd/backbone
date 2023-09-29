﻿using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Domain;
using MediatR;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;
public class Handler : IRequestHandler<UpdateClientCommand, UpdateClientResponse>
{
    private readonly IOAuthClientsRepository _oAuthClientsRepository;
    private readonly ITiersRepository _tiersRepository;

    public Handler(IOAuthClientsRepository oAuthClientsRepository, ITiersRepository tiersRepository)
    {
        _oAuthClientsRepository = oAuthClientsRepository;
        _tiersRepository = tiersRepository;
    }

    public async Task<UpdateClientResponse> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _oAuthClientsRepository.Find(request.ClientId, cancellationToken, track: true) ?? throw new NotFoundException(nameof(OAuthClient));

        var tierIdResult = TierId.Create(request.DefaultTier);
        if (tierIdResult.IsFailure)
            throw new ApplicationException(ApplicationErrors.Devices.InvalidTierIdOrDoesNotExist());

        var tierExists = await _tiersRepository.ExistsWithId(tierIdResult.Value, cancellationToken);
        if (!tierExists)
            throw new ApplicationException(ApplicationErrors.Devices.InvalidTierIdOrDoesNotExist());

        var changeError = client.ChangeDefaultTier(tierIdResult.Value);
        if (changeError != null)
            throw new DomainException(changeError);

        await _oAuthClientsRepository.Update(client, cancellationToken);

        return new UpdateClientResponse(client);
    }
}
