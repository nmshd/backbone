using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Tiers.CreateTier;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(t => t.Name)
            .Valid(TierName.Validate);
    }
}
