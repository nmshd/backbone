using Enmeshed.BuildingBlocks.Domain;

namespace Backbone.Modules.Quotas.Domain;
public interface IMetricCalculatorFactory
{
    public IMetricCalculator CreateFor(string metricKey);
}
