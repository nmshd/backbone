using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TokenCreated;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TokenCreated;

public class TokenCreatedDomainEventHandler : IDomainEventHandler<TokenCreatedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public TokenCreatedDomainEventHandler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(TokenCreatedDomainEvent @event)
    {
        var identities = new List<string> { @event.CreatedBy };
        var metrics = new List<string> { MetricKey.NumberOfTokens.Value };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, CancellationToken.None);
    }
}
