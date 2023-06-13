using Backbone.Modules.Quotas.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Application.Metrics;
public class MetricCalculatorFactory : IMetricCalculatorFactory
{
    private readonly IServiceProvider _services;

    public MetricCalculatorFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _services = serviceScopeFactory.CreateScope().ServiceProvider;
    }

    public IMetricCalculator CreateFor(string metricKey)
    {
        switch (metricKey)
        {
            case "NumberOfSentMessages":
                return _services.GetRequiredService<NumberOfSentMessagesMetricCalculator>();

            default:
                throw new NotSupportedException($"There is currently no {nameof(IMetricCalculator)} for the Metric with the key '{metricKey}'.");
        }
    }
}
