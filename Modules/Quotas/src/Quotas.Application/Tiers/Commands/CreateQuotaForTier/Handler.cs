using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using Microsoft.Extensions.Logging;

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
        _logger.LogInformation("Handling CreateQuotaForTierCommand ...");

        var tier = await _tiersRepository.Find(request.TierId, cancellationToken, true);

        var metricKey = (MetricKey)Enum.Parse(typeof(MetricKey), request.MetricKey);
        var metric = await _metricsRepository.Find(metricKey, cancellationToken); // ensure metric exists

        var result = tier.CreateQuota(metricKey, request.Max, request.Period);
        if (result.IsFailure)
            throw new OperationFailedException(GenericApplicationErrors.Validation.InvalidPropertyValue());

        await _tiersRepository.Update(tier, cancellationToken);

        _logger.LogTrace($"Successfully created assigned Quota to Tier. Tier ID: {tier.Id}, Tier Name: {tier.Name}");

        _eventBus.Publish(new QuotaCreatedForTierIntegrationEvent(tier.Id, result.Value.Id));

        _logger.LogTrace($"Successfully published QuotaCreatedForTierIntegrationEvent. Tier ID: {tier.Id}, Tier Name: {tier.Name}");

        var response = new TierQuotaDefinitionDTO(result.Value.Id, new MetricDTO(metric.Key, metric.DisplayName), result.Value.Max, result.Value.Period);
        return response;
    }
}
