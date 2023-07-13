using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteQuotaForTier;
public class DeleteQuotaForTierCommandValidator : AbstractValidator<DeleteQuotaForTierCommand>
{
    public DeleteQuotaForTierCommandValidator()
    {
        RuleFor(c => c.TierId).DetailedNotEmpty();
        RuleFor(c => c.TierQuotaDefinitionId).DetailedNotEmpty();
    }
}
