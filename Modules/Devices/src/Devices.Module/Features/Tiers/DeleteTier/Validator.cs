using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Tiers.DeleteTier;

public class Validator : AbstractValidator<DeleteTierCommand>
{
    public Validator()
    {
        RuleFor(c => c.TierId).ValidId<DeleteTierCommand, TierId>();
    }
}
