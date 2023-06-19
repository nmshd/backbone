using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Application.Metrics;
public class ServiceCollectionMetricCalculatorFactory : MetricCalculatorFactory
{
    private readonly IMessagesRepository _messagesRepository;

    public ServiceCollectionMetricCalculatorFactory(IMessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }

    public override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
    {
        return new NumberOfSentMessagesMetricCalculator(_messagesRepository);
    }
}
