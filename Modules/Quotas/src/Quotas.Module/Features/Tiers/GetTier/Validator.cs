using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentValidation;

namespace Backbone.Modules.Quotas.Module.Features.Tiers.GetTier;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(c => c.Id).ValidId<Query, TierId>();
    }
}
