using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
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
        var client = await _oAuthClientsRepository.Find(request.ClientId, cancellationToken);

        if (!string.IsNullOrEmpty(request.DefaultTier))
        {
            var tierIdResult = TierId.Create(request.DefaultTier);
            if (tierIdResult.IsFailure)
                throw new ApplicationException(ApplicationErrors.Devices.InvalidTierId());

            _ = await _tiersRepository.FindById(tierIdResult.Value, cancellationToken) ?? throw new ApplicationException(GenericApplicationErrors.NotFound(nameof(Tier)));
        }
        else
        {
            var basicTier = await _tiersRepository.GetBasicTierAsync(cancellationToken);
            request.DefaultTier = basicTier.Id.Value;
        }

        client.DefaultTier = request.DefaultTier;

        await _oAuthClientsRepository.Update(client, cancellationToken);

        return new UpdateClientResponse(client);
    }
}
