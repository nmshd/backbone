using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.FluentValidation;
using Enmeshed.BuildingBlocks.Domain.Errors;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;

public class CreateTierCommandValidator : AbstractDomainValidator<CreateTierCommand>
{
    public CreateTierCommandValidator()
    {
        RuleFor(t => t.Name)
            .Empty();
    }

    protected override IEnumerable<(DomainError?, string)> ValidateAgainstDomainModel(CreateTierCommand value)
    {
        yield return (TierName.Validate(value.Name), nameof(value.Name));
    }
}