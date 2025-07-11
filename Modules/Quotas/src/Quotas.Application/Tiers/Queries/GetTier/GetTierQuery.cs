using Backbone.Modules.Quotas.Application.DTOs;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTier;

public class GetTierQuery : IRequest<TierDetailsDTO>
{
    public required string Id { get; init; }
}
