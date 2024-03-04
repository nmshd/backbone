using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;

public class CreateTierCommandValidator : AbstractValidator<CreateTierCommand>
{
    public CreateTierCommandValidator()
    {
        RuleFor(t => t.Name)
            .Valid(TierName.Validate);
    }
}
