using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Metrics;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Application.Metrics;

public class ServiceProviderMetricCalculatorFactory : MetricCalculatorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceProviderMetricCalculatorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<NumberOfSentMessagesMetricCalculator>();
        return calculator;
    }

    protected override IMetricCalculator CreateNumberOfFilesMetricCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<NumberOfFilesMetricCalculator>();
        return calculator;
    }

    protected override IMetricCalculator CreateNumberOfRelationshipsMetricCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<NumberOfRelationshipsMetricCalculator>();
        return calculator;
    }

    protected override IMetricCalculator CreateNumberOfRelationshipTemplatesMetricCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<NumberOfRelationshipTemplatesMetricCalculator>();
        return calculator;
    }

    protected override IMetricCalculator CreateNumberOfTokensMetricCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<NumberOfTokensMetricCalculator>();
        return calculator;
    }

    protected override IMetricCalculator CreateUsedFileStorageSpaceCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<UsedFileStorageSpaceMetricCalculator>();
        return calculator;
    }

    protected override IMetricCalculator CreateNumberOfStartedDeletionProcessesCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<NumberOfStartedDeletionProcessesMetricCalculator>();
        return calculator;
    }

    protected override IMetricCalculator CreateNumberOfCreatedDatawalletModificationsCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<NumberOfCreatedDatawalletModificationsMetricCalculator>();
        return calculator;
    }

    protected override IMetricCalculator CreateNumberOfCreatedDevicesCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<NumberOfCreatedDevicesMetricCalculator>();
        return calculator;
    }

    protected override IMetricCalculator CreateNumberOfCreatedChallengesCalculator()
    {
        var calculator = _serviceProvider.GetRequiredService<NumberOfCreatedChallengesMetricCalculator>();
        return calculator;
    }
}
