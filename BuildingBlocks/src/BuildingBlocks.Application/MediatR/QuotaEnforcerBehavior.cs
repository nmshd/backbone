using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using MediatR;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

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
        var attrs = request.GetType().CustomAttributes;

        foreach (var attr in attrs)
        {
            if (attr.AttributeType == typeof(ApplyQuotasForMetricsAttribute))
            {
                var args = attr.ConstructorArguments.Select(it => new MetricKey(it.Value as string)).ToList();
                var statuses = await _metricStatusesRepository.GetMetricStatuses(_userContext.GetAddress(), args);
                var expiredStatuses = statuses.Where(it=> it.IsExhaustedUntil < DateTime.Now).ToList();
                // for each of the arguments, determine if they're exhausted.
                // if one or more are exhausted, an exception should be thrown.
                // We're to determine which MetricKey to pass to the expection
                // DPS: suggestion that we pass the one with the bigger `IsExhaustedUntil` (so that actions submited past that point are more likely to succeed).
                if (expiredStatuses.Any())
                {
                    var mostInTheFuture = expiredStatuses.MaxBy(it => it.IsExhaustedUntil);
                    if (mostInTheFuture != null)
                    {
                        throw new QuotaExhaustedException(mostInTheFuture.MetricKey, mostInTheFuture.IsExhaustedUntil);
                    }
                }
            }
        }
        var response = await next();
        return response;
    }
}
