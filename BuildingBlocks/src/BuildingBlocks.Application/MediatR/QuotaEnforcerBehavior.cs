using Enmeshed.Common.Infrastructure.Persistence.Repository;
using MediatR;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

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
        
        var response = await next();
        return response;
    }
}
