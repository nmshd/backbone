using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public class EventBusHealthCheck : IHealthCheck
{
    private readonly IEventBus _eventBus;

    public EventBusHealthCheck(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        var result = _eventBus.IsConnected;
        return Task.FromResult(result ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
    }
}
