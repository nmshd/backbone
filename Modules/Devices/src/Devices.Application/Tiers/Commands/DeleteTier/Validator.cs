using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;

public class Validator : AbstractValidator<DeleteTierCommand>
{
    public Validator()
    {
        RuleFor(c => c.TierId).ValidId<DeleteTierCommand, TierId>();
    }
}
