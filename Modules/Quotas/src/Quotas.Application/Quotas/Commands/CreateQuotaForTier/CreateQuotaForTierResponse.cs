using Backbone.Modules.Quotas.Application.Quotas.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Quotas.Commands.CreateQuotaForTier;

public class CreateQuotaForTierResponse : TierQuotaDefinitionDTO
{
    public CreateQuotaForTierResponse(TierQuotaDefinitionId id, Metric metric, int max, QuotaPeriod period) : base(id, metric, max, period)
    {
    }
}
