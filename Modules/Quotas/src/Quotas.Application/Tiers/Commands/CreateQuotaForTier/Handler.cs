using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForTier;

public class Handler : IRequestHandler<CreateQuotaForTierCommand, TierQuotaDefinitionDTO>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IEventBus _eventBus;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(ITiersRepository tierRepository, ILogger<Handler> logger, IEventBus eventBus, IMetricsRepository metricsRepository)
    {
        _tiersRepository = tierRepository;
        _logger = logger;
        _eventBus = eventBus;
        _metricsRepository = metricsRepository;
    }

    public async Task<TierQuotaDefinitionDTO> Handle(CreateQuotaForTierCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Handling CreateQuotaForTierCommand ...");

        var tier = await _tiersRepository.Find(request.TierId, cancellationToken, true);

        var parseMetricKeyResult = MetricKey.Parse(request.MetricKey);

        if (parseMetricKeyResult.IsFailure)
            throw new DomainException(parseMetricKeyResult.Error);

        var metric = await _metricsRepository.Find(parseMetricKeyResult.Value, cancellationToken);

        var result = tier.CreateQuota(parseMetricKeyResult.Value, request.Max, request.Period);
        if (result.IsFailure)
            throw new DomainException(result.Error);

        await _tiersRepository.Update(tier, cancellationToken);

        CreateQuotaForTierLogs.CreatedQuotaForTier(_logger, tier.Id, tier.Name);

        _eventBus.Publish(new QuotaCreatedForTierIntegrationEvent(tier.Id, result.Value.Id));

        var response = new TierQuotaDefinitionDTO(result.Value.Id, new MetricDTO(metric), result.Value.Max, result.Value.Period);
        return response;
    }
}

internal static partial class CreateQuotaForTierLogs
{
    [LoggerMessage(
        EventId = 346835,
        EventName = "Quotas.CreatedQuotaForTier",
        Level = LogLevel.Information,
        Message = "Successfully created Quota for Tier. Tier ID: '{tierId}', Tier Name: '{tierName}'.")]
    public static partial void CreatedQuotaForTier(ILogger logger, TierId tierId, string tierName);
}
