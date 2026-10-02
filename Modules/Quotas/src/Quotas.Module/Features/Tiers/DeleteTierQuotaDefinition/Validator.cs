using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentValidation;

namespace Backbone.Modules.Quotas.Module.Features.Tiers.DeleteTierQuotaDefinition;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.TierId).ValidId<Command, TierId>();
        RuleFor(c => c.TierQuotaDefinitionId).ValidId<Command, TierQuotaDefinitionId>();
    }
}
