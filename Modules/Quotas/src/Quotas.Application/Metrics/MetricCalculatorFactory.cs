using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Application.Metrics;
public class MetricCalculatorFactory : IMetricCalculatorFactory
{
    private readonly IServiceProvider _services;

    public MetricCalculatorFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _services = serviceScopeFactory.CreateScope().ServiceProvider;
    }

    public IMetricCalculator CreateFor(MetricKey metricKey)
    {
        if (metricKey == MetricKey.NumberOfSentMessages)
        {
            return _services.GetRequiredService<NumberOfSentMessagesMetricCalculator>();
        }

        throw new NotSupportedException($"There is currently no {nameof(IMetricCalculator)} for the Metric with the key '{metricKey}'.");
    }
}
