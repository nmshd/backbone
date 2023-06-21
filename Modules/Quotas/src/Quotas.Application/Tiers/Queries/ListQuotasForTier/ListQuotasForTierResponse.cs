using Backbone.Modules.Quotas.Application.DTOs;
using Enmeshed.BuildingBlocks.Application.CQRS.BaseClasses;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.ListQuotaForTier;
public class ListQuotasForTierResponse : EnumerableResponseBase<TierQuotaDefinitionDTO>
{
    public ListQuotasForTierResponse(IEnumerable<TierQuotaDefinitionDTO> items) : base(items) { }
}
