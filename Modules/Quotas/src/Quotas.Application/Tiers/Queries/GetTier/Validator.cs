using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTier;

public class Validator : AbstractValidator<GetTierQuery>
{
    public Validator()
    {
        RuleFor(c => c.Id).ValidId<GetTierQuery, TierId>();
    }
}
