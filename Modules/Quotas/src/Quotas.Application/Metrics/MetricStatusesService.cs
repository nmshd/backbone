using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Metrics;
using Enmeshed.BuildingBlocks.Domain;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Metrics;

public class MetricStatusesService
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;

    public MetricStatusesService(MetricCalculatorFactory metricCalculatorFactory, IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task RecalculateMetricStatuses(List<string> identityAddresses, List<string> metrics, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.FindByAddresses(identityAddresses, cancellationToken, track: true);
        foreach (var identity in identities)
        {
            await identity.UpdateMetricStatuses(ParseMetricKeys(metrics), _metricCalculatorFactory, cancellationToken);
        }

        await _identitiesRepository.Update(identities, cancellationToken);
    }

    private static IEnumerable<MetricKey> ParseMetricKeys(IEnumerable<string> metricKeys)
    {
        var parsedMetricKeys = new List<MetricKey>();

        foreach (var metricKey in metricKeys)
        {
            var parseResult = MetricKey.Parse(metricKey);

            if (parseResult.IsFailure)
                throw new DomainException(parseResult.Error);

            parsedMetricKeys.Add(parseResult.Value);
        }

        return parsedMetricKeys;
    }
}
