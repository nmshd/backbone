using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;

public class DeleteTierQuotaDefinitionCommand : IRequest
{
    public required string TierId { get; init; }
    public required string TierQuotaDefinitionId { get; init; }
}
