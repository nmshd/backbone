using Backbone.Modules.Quotas.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Tiers.GetTier;

public class GetTierQuery : IRequest<TierDetailsDTO>
{
    public required string Id { get; init; }
}
