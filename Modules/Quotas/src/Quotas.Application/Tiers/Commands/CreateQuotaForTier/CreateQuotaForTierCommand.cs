using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForTier;

public class CreateQuotaForTierCommand : IRequest<TierQuotaDefinitionDTO>
{
    public CreateQuotaForTierCommand(string tierId, string metricKey, int max, QuotaPeriod period)
    {
        TierId = tierId;
        MetricKey = metricKey;
        Max = max;
        Period = period;
    }

    public string TierId { get; set; }
    public string MetricKey { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}
