using Backbone.Modules.Quotas.Application.Quotas.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.Quotas.Commands.CreateQuotaForTier;

public class CreateQuotaForTierResponse : TierQuotaDefinitionDTO
{
    public CreateQuotaForTierResponse(string id, Metric metric, int max, QuotaPeriod period) : base(id, metric, max, period)
    {
    }
}
