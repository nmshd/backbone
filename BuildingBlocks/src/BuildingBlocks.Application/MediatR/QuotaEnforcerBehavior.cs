using System.Collections.ObjectModel;
using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Attributes;
using Backbone.BuildingBlocks.Application.QuotaCheck;
using Backbone.BuildingBlocks.Domain;
using MediatR;

namespace Backbone.BuildingBlocks.Application.MediatR;

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
            foreach (var customAttributeTypedArgument in applyQuotasForMetricsAttribute.ConstructorArguments)
            {
                foreach (var element in (ReadOnlyCollection<CustomAttributeTypedArgument>)customAttributeTypedArgument.Value!)
                {
                    metricKeys.Add(new MetricKey((element.Value as string)!));
                }
            }

            var result = await _quotaChecker.CheckQuotaExhaustion(metricKeys.AsEnumerable());

            if (!result.IsSuccess)
            {
                throw new QuotaExhaustedException(result.ExhaustedStatuses.ToArray());
            }
        }

        var response = await next(cancellationToken);
        return response;
    }
}
