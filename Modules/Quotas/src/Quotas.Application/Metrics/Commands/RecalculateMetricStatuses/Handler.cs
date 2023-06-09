using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Metrics.Commands.RecalculateMetricStatuses;
public class Handler : IRequestHandler<RecalculateMetricStatusesCommand>
{
    private readonly IMetricCalculatorFactory _metricCalculatorFactory;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IMetricCalculatorFactory metricCalculatorFactory, IIdentitiesRepository identitiesRepository, IQuotasRepository quotasRepository)
    {
        _metricCalculatorFactory = metricCalculatorFactory;
        _identitiesRepository = identitiesRepository;
        _quotasRepository = quotasRepository;
    }

    public async Task Handle(RecalculateMetricStatusesCommand command, CancellationToken cancellationToken)
    {
        foreach (var identityId in command.Identities)
        {
            var identity = await _identitiesRepository.FindById(identityId, cancellationToken); 
            await identity.UpdateMetrics(command.Metrics, _metricCalculatorFactory, _quotasRepository, cancellationToken);
        }

        //var metricCalculator = _metricCalculatorFactory.CreateFor()
            throw new NotImplementedException();
    }
}
