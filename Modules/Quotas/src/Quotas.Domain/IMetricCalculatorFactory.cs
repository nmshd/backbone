
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain;
public interface IMetricCalculatorFactory
{
    public IMetricCalculator CreateFor(MetricKey metricKey);
}
