using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using MediatR;

namespace Enmeshed.BuildingBlocks.Application.MediatR;
public class QuotaEnforcerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMetricStatusesRepository? _metricStatusesRepository;

    public QuotaEnforcerBehavior(IMetricStatusesRepository metricStatusesRepositories)
    {
        _metricStatusesRepository = metricStatusesRepositories;
    }

    public QuotaEnforcerBehavior()
    {
        _metricStatusesRepository = null;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Using reflection.
        var attrs = request.GetType().CustomAttributes;  // Reflection.

        // Displaying output.
        foreach (var attr in attrs)
        {
            if (attr.AttributeType == typeof(ApplyQuotasForMetricsAttribute))
            {
                //attr.ConstructorArguments.Select(it=> new MetricKey())
                var args = attr.ConstructorArguments.Select(it => it.Value).ToList();
                if (args.Any())
                {
                    Console.WriteLine("Yay");
                }
            }
        }
        var response = await next();
        return response;
    }
}
