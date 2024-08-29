using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;

public class Validator : AbstractValidator<GetTierByIdQuery>
{
    public Validator()
    {
        RuleFor(c => c.Id).ValidId<GetTierByIdQuery, TierId>();
    }
}
