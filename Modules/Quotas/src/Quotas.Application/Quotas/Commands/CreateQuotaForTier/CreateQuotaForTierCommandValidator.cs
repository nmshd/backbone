using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Quotas.Application.Quotas.Commands.CreateQuotaForTier;

public class CreateQuotaForTierCommandValidator : AbstractValidator<CreateQuotaForTierCommand>
{
    public CreateQuotaForTierCommandValidator()
    {
        RuleFor(c => c.TierId)
            .DetailedNotEmpty();
        RuleFor(c => c.Max)
            .GreaterThan(0);
        RuleFor(c => c.Period)
            .NotNull()
            .IsInEnum();
        RuleFor(c => c.Metric)
            .NotNull();
    }
}
