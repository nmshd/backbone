using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Tiers.DeleteTierQuotaDefinition;

public class DeleteTierQuotaDefinitionCommand : IRequest
{
    public required string TierId { get; init; }
    public required string TierQuotaDefinitionId { get; init; }
}
