using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForIdentity;

public class Handler : IRequestHandler<CreateQuotaForIdentityCommand, IdentityQuotaDefinitionDTO>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IEventBus _eventBus;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(IIdentitiesRepository identitiesRepository, ILogger<Handler> logger, IEventBus eventBus, IMetricsRepository metricsRepository)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
        _eventBus = eventBus;
        _metricsRepository = metricsRepository;
    }

    public async Task<IdentityQuotaDefinitionDTO> Handle(CreateQuotaForIdentityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateQuotaForTierCommand ...");

        var identity = await _identitiesRepository.FindByAddress(request.IdentityAddress, cancellationToken, true);

        var metricKey = (MetricKey)Enum.Parse(typeof(MetricKey), request.MetricKey);
        var metric = await _metricsRepository.Find(metricKey, cancellationToken); // ensure metric exists

        var individualQuota = identity.CreateIndividualQuota(metricKey, request.Max, request.Period);

        await _identitiesRepository.Update(identity, cancellationToken);

        _logger.LogTrace($"Successfully created assigned Quota to Identity. Identity Address: {identity.Address}");

        var response = new IdentityQuotaDefinitionDTO(individualQuota.Id, new MetricDTO(metric.Key, metric.DisplayName), individualQuota.Max, individualQuota.Period);
        return response;
    }
}
