using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using MediatR;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

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

        var changeDefaultTierError = client.ChangeDefaultTier(tierIdResult.Value);
        var changeMaxIdentitiesError = client.ChangeMaxIdentities(request.MaxIdentities);

        if (CanUpdate(new List<DomainError> {changeMaxIdentitiesError, changeMaxIdentitiesError}))
        {
            await _oAuthClientsRepository.Update(client, cancellationToken);
        }
        else
        {
            if (changeDefaultTierError != null)
                throw new DomainException(changeDefaultTierError);
            if (changeMaxIdentitiesError != null)
                throw new DomainException(changeMaxIdentitiesError);
        }

        return new UpdateClientResponse(client);
    }

    public static bool CanUpdate(List<DomainError> fields)
    {
        return fields.Any(f => f == null);
    }
}
