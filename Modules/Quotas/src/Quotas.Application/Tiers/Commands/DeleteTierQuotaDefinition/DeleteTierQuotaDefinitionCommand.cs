using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;
public class DeleteTierQuotaDefinitionCommand : IRequest
{
    public DeleteTierQuotaDefinitionCommand(string tierId, string tierQuotaDefinitionId)
    {
        TierId = tierId;
        TierQuotaDefinitionId = tierQuotaDefinitionId;
    }

    public string TierId { get; set; }
    public string TierQuotaDefinitionId { get; set; }
}
