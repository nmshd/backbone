using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<PushDatawalletModificationsCommand>
{
    public Validator()
    {
        RuleFor(r => r.Modifications).DetailedNotEmpty();
        RuleForEach(r => r.Modifications).SetValidator(new PushDatawalletModificationItemValidator());
        RuleFor(r => r.SupportedDatawalletVersion).Must(v => v > 0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("'SupportedDatawalletVersion' must be greater than 0.");
    }
}
