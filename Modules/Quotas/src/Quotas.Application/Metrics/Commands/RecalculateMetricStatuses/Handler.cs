using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Metrics;
using Enmeshed.BuildingBlocks.Domain;
using MediatR;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Metrics.Commands.RecalculateMetricStatuses;
public class Handler : IRequestHandler<RecalculateMetricStatusesCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;

    public Handler(MetricCalculatorFactory metricCalculatorFactory, IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
        _metricCalculatorFactory = metricCalculatorFactory; 
    }

    public async Task Handle(RecalculateMetricStatusesCommand command, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.FindByAddresses(command.Identities, cancellationToken, track: true);
        
        foreach (var identity in identities)
        {
            await identity.UpdateMetricStatuses(ParseMetricKeys(command.Metrics), _metricCalculatorFactory, cancellationToken);
        }

        await _identitiesRepository.Update(identities, cancellationToken);
    }

    private static IEnumerable<MetricKey> ParseMetricKeys(IEnumerable<string> metricKeys)
    {
        var parsedMetricKeys = new List<MetricKey>();

        foreach (var metricKey in metricKeys)
        {
            var parseResult = MetricKey.Parse(metricKey);

            if(parseResult.IsFailure)
                throw new DomainException(parseResult.Error);

            parsedMetricKeys.Add(parseResult.Value);
        }

        return parsedMetricKeys;
    }
}
