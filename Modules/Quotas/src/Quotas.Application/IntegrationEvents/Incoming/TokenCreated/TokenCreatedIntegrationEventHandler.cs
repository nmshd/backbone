using Backbone.Modules.Quotas.Application.Metrics.Commands.RecalculateMetricStatuses;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TokenCreated;
public class TokenCreatedIntegrationEventHandler : IIntegrationEventHandler<TokenCreatedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public TokenCreatedIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(TokenCreatedIntegrationEvent @event)
    {
        var identities = new List<string> { @event.CreatedBy };
        var metrics = new List<string> { MetricKey.NumberOfTokens.Value };

        await _mediator.Send(new RecalculateMetricStatusesCommand(identities, metrics));
    }
}
