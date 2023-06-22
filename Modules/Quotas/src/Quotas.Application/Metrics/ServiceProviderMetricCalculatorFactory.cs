using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
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
        var messagesRepository = _serviceProvider.GetService<IMessagesRepository>();
        return new NumberOfSentMessagesMetricCalculator(messagesRepository);
    }
}
