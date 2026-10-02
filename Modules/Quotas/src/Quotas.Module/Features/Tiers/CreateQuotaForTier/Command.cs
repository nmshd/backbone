using Backbone.Modules.Quotas.Module.Features.Shared;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Tiers.CreateQuotaForTier;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateQuotaForTierCommand")]
public class Command : IRequest<TierQuotaDefinitionDTO>
{
    public required string TierId { get; init; }
    public required string MetricKey { get; init; }
    public required int Max { get; init; }
    public required QuotaPeriod Period { get; init; }
}
