using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using MediatR;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.Tooling;
using Enmeshed.BuildingBlocks.Domain;

namespace Enmeshed.BuildingBlocks.Application.MediatR;
public class QuotaEnforcerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
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

        var relevantAttribute = attributes.FirstOrDefault(attribute => attribute.AttributeType == typeof(ApplyQuotasForMetricsAttribute));
        if (relevantAttribute != null)
        {
            var metricKeys = relevantAttribute.ConstructorArguments.Select(it => new MetricKey(it.Value as string)).ToList();

            var statuses = await _metricStatusesRepository.GetMetricStatuses(_userContext.GetAddress(), metricKeys);

            var expiredStatuses = statuses.Where(it=> it.IsExhaustedUntil < SystemTime.UtcNow).ToList();
            
            if (expiredStatuses.Any())
            {
                var mostInTheFuture = expiredStatuses.MaxBy(it => it.IsExhaustedUntil);
                if (mostInTheFuture != null)
                {
                    throw new QuotaExhaustedException(mostInTheFuture.MetricKey, mostInTheFuture.IsExhaustedUntil);
                }
            }
        }
         
        var response = await next();
        return response;
    }
}
