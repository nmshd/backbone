using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Metrics;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Metrics;

public class MetricStatusesService : IMetricStatusesService
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;

    public MetricStatusesService(MetricCalculatorFactory metricCalculatorFactory, IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task RecalculateMetricStatuses(List<string> identityAddresses, List<MetricKey> metrics, MetricUpdateType updateType, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.FindByAddresses(identityAddresses, cancellationToken, track: true);
        foreach (var identity in identities)
        {
            await identity.UpdateMetricStatuses(metrics, _metricCalculatorFactory, updateType, cancellationToken);
        }

        await _identitiesRepository.Update(identities, cancellationToken);
    }
}

public interface IMetricStatusesService
{
    Task RecalculateMetricStatuses(List<string> identityAddresses, List<MetricKey> metrics, MetricUpdateType updateType, CancellationToken cancellationToken);
}
