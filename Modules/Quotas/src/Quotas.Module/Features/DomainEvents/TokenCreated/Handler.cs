using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Module.Features.Metrics.Shared;
using Backbone.Modules.Tokens.Contracts.DomainEvents;

namespace Backbone.Modules.Quotas.Module.Features.DomainEvents.TokenCreated;

public class Handler : IDomainEventHandler<TokenCreatedDomainEvent>
{
    private readonly IMetricStatusesService _metricStatusesService;

    public Handler(IMetricStatusesService metricStatusesService)
    {
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(TokenCreatedDomainEvent @event)
    {
        if (@event.CreatedBy == null)
            return;

        var identities = new List<string> { @event.CreatedBy };
        var metrics = new List<MetricKey> { MetricKey.NUMBER_OF_TOKENS };

        await _metricStatusesService.RecalculateMetricStatuses(identities, metrics, MetricUpdateType.All, CancellationToken.None);
    }
}
