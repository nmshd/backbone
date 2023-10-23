using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Devices.Application.Tiers.Commands.DeleteTier;

public class DeleteTierCommandValidator : AbstractValidator<DeleteTierCommand>
{
    public DeleteTierCommandValidator()
    {
        RuleFor(c => c.TierId).DetailedNotEmpty();
    }
}
