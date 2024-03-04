using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForTier;

public class CreateQuotaForTierCommandValidator : AbstractValidator<CreateQuotaForTierCommand>
{
    public CreateQuotaForTierCommandValidator()
    {
        RuleFor(c => c.TierId)
            .DetailedNotEmpty();
        RuleFor(c => c.Max)
            .GreaterThan(0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        RuleFor(c => c.Period)
            .DetailedNotNull();
        RuleFor(c => c.MetricKey)
            .DetailedNotNull();
    }
}
