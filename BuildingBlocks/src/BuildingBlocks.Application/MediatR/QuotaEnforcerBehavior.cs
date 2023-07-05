using MediatR;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Application.QuotaCheck;
using System.Reflection;
using System.Collections.ObjectModel;

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
            var metricKeys = new List<MetricKey>();
            foreach ( var customAttributeTypedArgument in applyQuotasForMetricsAttribute.ConstructorArguments) {
                foreach (var element in (ReadOnlyCollection<CustomAttributeTypedArgument>) customAttributeTypedArgument.Value)
                {
                    metricKeys.Add(new MetricKey(element.Value as string));
                }
            }

            var result = await _quotaChecker.CheckQuotaExhaustion(metricKeys.AsEnumerable());

            if (!result.IsSuccess)
            {
                throw new QuotaExhaustedException(result.ExhaustedStatuses.ToArray());
            }
        }

        var response = await next();
        return response;
    }
}
