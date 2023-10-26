﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Domain;
using Backbone.Common.Infrastructure.Persistence.Repository;

namespace Backbone.BuildingBlocks.Application.QuotaCheck;
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
