using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;

internal class Validator : AbstractValidator<StartSyncRunCommand>
{
    public Validator()
    {
        RuleFor(r => r.SupportedDatawalletVersion).Must(v => v > 0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("'SupportedDatawalletVersion' must be greater than 0.");
    }
}
