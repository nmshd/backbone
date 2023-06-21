using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.ListQuotaForTier;
public class ListQuotasForTierQuery : IRequest<ListQuotasForTierResponse>
{
    public ListQuotasForTierQuery(string tierId)
    {
        TierId = tierId;
    }

    public string TierId { get; set; }
}
