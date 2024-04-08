using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using MediatR;
using Microsoft.Extensions.Logging;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

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

        _logger.CreatedTier(tier.Id.Value, tier.Name.Value);

        _eventBus.Publish(new TierCreatedIntegrationEvent(tier));

        return new CreateTierResponse(tier.Id, tier.Name);
    }
}

internal static partial class CreateTierLogs
{
    [LoggerMessage(
        EventId = 383136,
        EventName = "Devices.CreateTier.CreatedTier",
        Level = LogLevel.Information,
        Message = "Successfully created tier. Tier ID: '{tierId}', Tier Name: {tierName}")]
    public static partial void CreatedTier(this ILogger logger, string tierId, string tierName);
}
