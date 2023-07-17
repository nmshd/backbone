using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;
public class GetTierByIdResponse : TierDetailsDTO
{
    public GetTierByIdResponse(Tier tier, IEnumerable<Metric> metrics) : base(tier, metrics) { }
}
