using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteQuotaForTier;
public class DeleteQuotaForTierCommand : IRequest
{
    public DeleteQuotaForTierCommand(string tierId, string tierQuotaDefinitionId)
    {
        TierId = tierId;
        TierQuotaDefinitionId = tierQuotaDefinitionId;
    }

    public string TierId { get; set; }
    public string TierQuotaDefinitionId { get; set; }
}
