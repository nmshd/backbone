using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using Microsoft.Extensions.Logging;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;

public class Handler : IRequestHandler<CreateTierCommand, CreateTierResponse>
{
    private readonly ITiersRepository _tierRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IEventBus _eventBus;

    public Handler(ITiersRepository tierRepository, ILogger<Handler> logger, IEventBus eventBus)
    {
        _tierRepository = tierRepository;
        _logger = logger;
        _eventBus = eventBus;
    }

    public async Task<CreateTierResponse> Handle(CreateTierCommand request, CancellationToken cancellationToken)
    {
        var tierName = TierName.Create(request.Name);

        var tierExists = await _tierRepository.ExistsWithName(tierName.Value, cancellationToken);
        if (tierExists)
            throw new ApplicationException(ApplicationErrors.Devices.TierNameAlreadyExists());

        var tier = new Tier(tierName.Value);

        await _tierRepository.AddAsync(tier, cancellationToken);

        _logger.LogTrace("Successfully created tier. Tier ID: {tierId}, Tier Name: {tierName}", tier.Id.Value, tier.Name.Value);

        _eventBus.Publish(new TierCreatedIntegrationEvent(tier));

        _logger.LogTrace("Successfully published TierCreatedIntegrationEvent. Tier ID: {tierId}, Tier Name: {tierName}", tier.Id.Value, tier.Name.Value);

        return new CreateTierResponse(tier.Id, tier.Name);
    }
}
