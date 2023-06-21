using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.QuotaCheck;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.Common.Infrastructure.Persistence.Repository;

namespace Backbone.Modules.Quotas.Application.QuotaCheck;
public class QuotaCheckerImpl : IQuotaChecker
{
    private readonly IUserContext _userContext;
    private readonly IMetricStatusesRepository _metricStatusesRepository;

    public QuotaCheckerImpl(IUserContext userContext, IMetricStatusesRepository metricStatusesRepository)
    {
        _userContext = userContext;
        _metricStatusesRepository = metricStatusesRepository;
    }

    public async Task<CheckQuotaResult> CheckQuotaExhaustion(IEnumerable<MetricKey> metricKeys)
    {
        var statuses = await _metricStatusesRepository.GetMetricStatuses(_userContext.GetAddress(), metricKeys);

        var exhaustedStatuses = statuses.Where(m => m.IsExhausted);

        return new CheckQuotaResult(exhaustedStatuses);
    }
}
