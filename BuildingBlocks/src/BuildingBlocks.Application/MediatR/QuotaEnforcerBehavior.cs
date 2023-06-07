using Enmeshed.Common.Infrastructure.Persistence.Repository;
using MediatR;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Domain;

namespace Enmeshed.BuildingBlocks.Application.MediatR;
public class QuotaEnforcerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IMetricStatusesRepository _metricStatusesRepository;
    private readonly IUserContext _userContext;

    public QuotaEnforcerBehavior(IMetricStatusesRepository metricStatusesRepositories, IUserContext userContext)
    {
        _metricStatusesRepository = metricStatusesRepositories;
        _userContext = userContext;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attributes = request.GetType().CustomAttributes;

        var applyQuotasForMetricsAttribute = attributes.FirstOrDefault(attribute => attribute.AttributeType == typeof(ApplyQuotasForMetricsAttribute));
        if (applyQuotasForMetricsAttribute != null)
        {
            var metricKeys = applyQuotasForMetricsAttribute.ConstructorArguments.Select(it => new MetricKey(it.Value as string)).ToList();

            var statuses = await _metricStatusesRepository.GetMetricStatuses(_userContext.GetAddress(), metricKeys);

            var exhaustedStatuses = statuses.Where(m => m.IsExhausted).ToList();

            if (exhaustedStatuses.Any())
            {
                throw new QuotaExhaustedException(exhaustedStatuses.ToArray());
            }
        }

        var response = await next();
        return response;
    }
}
