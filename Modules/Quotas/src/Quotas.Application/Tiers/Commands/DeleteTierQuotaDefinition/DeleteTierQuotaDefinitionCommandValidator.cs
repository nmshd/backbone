using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;
public class DeleteTierQuotaDefinitionCommandValidator : AbstractValidator<DeleteTierQuotaDefinitionCommand>
{
    public DeleteTierQuotaDefinitionCommandValidator()
    {
        RuleFor(c => c.TierId).DetailedNotEmpty();
        RuleFor(c => c.TierQuotaDefinitionId).DetailedNotEmpty();
    }
}
