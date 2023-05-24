using Backbone.Modules.Quotas.Application.Quotas.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Quotas.Commands.CreateQuotaForTier;

public class CreateQuotaForTierCommand : IRequest<TierQuotaDefinitionDTO>
{
    public CreateQuotaForTierCommand(string tierId, MetricKey metricKey, int max, QuotaPeriod period)
    {
        TierId = tierId;
        MetricKey = metricKey;
        Max = max;
        Period = period;
    }

    public string TierId { get; set; }
    public MetricKey MetricKey { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}
