using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Application.Metrics;
public class MetricCalculatorFactory
{
    private readonly IServiceProvider _services;

    public MetricCalculatorFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _services = serviceScopeFactory.CreateScope().ServiceProvider;
    }

    public IMetricCalculator CreateFor(MetricKey metricKey)
    {
        switch (metricKey)
        {
            case MetricKey.NumberOfSentMessages:
                return _services.GetRequiredService<NumberOfSentMessagesCalculator>();

            default:
                throw new Exception();
        }
    }
}
