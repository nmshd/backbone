using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TokenCreated;

public class TokenCreatedIntegrationEventHandler : IIntegrationEventHandler<TokenCreatedIntegrationEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public TokenCreatedIntegrationEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(TokenCreatedIntegrationEvent @event)
    {
        var identities = new List<string> { @event.CreatedBy };
        var metrics = new List<string> { MetricKey.NumberOfTokens.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
