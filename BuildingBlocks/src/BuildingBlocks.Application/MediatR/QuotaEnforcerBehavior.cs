using MediatR;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Application.QuotaCheck;

namespace Enmeshed.BuildingBlocks.Application.MediatR;
public class QuotaEnforcerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IQuotaChecker _quotaChecker;

    public QuotaEnforcerBehavior(IQuotaChecker quotaChecker)
    {
        _quotaChecker = quotaChecker;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        
        var attributes = request.GetType().CustomAttributes;

        var applyQuotasForMetricsAttribute = attributes.FirstOrDefault(attribute => attribute.AttributeType == typeof(ApplyQuotasForMetricsAttribute));
        if (applyQuotasForMetricsAttribute != null)
        {
            var metricKeys = applyQuotasForMetricsAttribute.ConstructorArguments.Select(it => new MetricKey(it.Value as string)).ToList();

            var result = await _quotaChecker.CheckQuotaExhaustion(metricKeys);

            if (!result.IsSuccess)
            {
                throw new QuotaExhaustedException(result.ExhaustedStatuses.ToArray());
            }
        }

        var response = await next();
        return response;
    }
}
