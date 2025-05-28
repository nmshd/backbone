using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using MediatR;
using Microsoft.Extensions.Logging;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForTier;

public class Handler : IRequestHandler<CreateQuotaForTierCommand, TierQuotaDefinitionDTO>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(ITiersRepository tierRepository, ILogger<Handler> logger, IMetricsRepository metricsRepository)
    {
        _tiersRepository = tierRepository;
        _logger = logger;
        _metricsRepository = metricsRepository;
    }

    public async Task<TierQuotaDefinitionDTO> Handle(CreateQuotaForTierCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Handling CreateQuotaForTierCommand ...");

        var tier = await _tiersRepository.Get(request.TierId, cancellationToken, true) ?? throw new NotFoundException(nameof(Tier));

        var parseMetricKeyResult = MetricKey.Parse(request.MetricKey);

        if (parseMetricKeyResult.IsFailure)
            throw new DomainException(parseMetricKeyResult.Error);

        var metric = await _metricsRepository.Get(parseMetricKeyResult.Value, cancellationToken);

        var result = tier.CreateQuota(parseMetricKeyResult.Value, request.Max, request.Period);
        if (result.IsFailure)
            throw new DomainException(result.Error);

        await _tiersRepository.Update(tier, cancellationToken);

        _logger.CreatedQuotaForTier(tier.Id, tier.Name, result.Value.Id);

        var response = new TierQuotaDefinitionDTO(result.Value.Id, new MetricDTO(metric), result.Value.Max, result.Value.Period);
        return response;
    }
}

internal static partial class CreateQuotaForTierLogs
{
    [LoggerMessage(
        EventId = 346835,
        EventName = "Quotas.CreateQuotaForTier.CreatedQuotaForTier",
        Level = LogLevel.Information,
        Message = "Successfully created Quota for Tier. Tier ID: '{tierId}', Tier Name: '{tierName}', Quota Definition ID: {quotaDefinitionId}.")]
    public static partial void CreatedQuotaForTier(this ILogger logger, TierId tierId, string tierName, string quotaDefinitionId);
}
