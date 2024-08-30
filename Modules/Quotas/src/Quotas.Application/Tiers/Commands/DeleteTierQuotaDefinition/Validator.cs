using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;

public class Validator : AbstractValidator<DeleteTierQuotaDefinitionCommand>
{
    public Validator()
    {
        RuleFor(c => c.TierId).ValidId<DeleteTierQuotaDefinitionCommand, TierId>();
        RuleFor(c => c.TierQuotaDefinitionId).ValidId<DeleteTierQuotaDefinitionCommand, TierQuotaDefinitionId>();
    }
}
