using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Tiers.DeleteTierQuotaDefinition;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteTierQuotaDefinitionCommand")]
public class Command : IRequest
{
    public required string TierId { get; init; }
    public required string TierQuotaDefinitionId { get; init; }
}
