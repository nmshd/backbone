using Backbone.Modules.Quotas.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Application.Metrics;
public class ServiceProviderMetricCalculatorFactory : MetricCalculatorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceProviderMetricCalculatorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<NumberOfSentMessagesMetricCalculator>();
        return calculator;
    }
}
