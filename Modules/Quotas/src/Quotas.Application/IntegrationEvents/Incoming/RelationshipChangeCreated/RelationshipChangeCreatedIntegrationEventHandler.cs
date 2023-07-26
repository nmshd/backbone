using Backbone.Modules.Quotas.Application.Metrics.Commands.RecalculateMetricStatuses;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipChangeCreated;
public class RelationshipChangeCreatedIntegrationEventHandler : IIntegrationEventHandler<RelationshipChangeCreatedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public RelationshipChangeCreatedIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(RelationshipChangeCreatedIntegrationEvent @event)
    {
        var identities = new List<string> { @event.ChangeCreatedBy, @event.ChangeRecipient };
        var metrics = new List<string> { MetricKey.NumberOfRelationships.Value };

        await _mediator.Send(new RecalculateMetricStatusesCommand(identities, metrics));
    }
}

