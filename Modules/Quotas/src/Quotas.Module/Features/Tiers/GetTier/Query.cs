using Backbone.Modules.Quotas.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Tiers.GetTier;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetTierQuery")]
public class Query : IRequest<TierDetailsDTO>
{
    public required string Id { get; init; }
}
