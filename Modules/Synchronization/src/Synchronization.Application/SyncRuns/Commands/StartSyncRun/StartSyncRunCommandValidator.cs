using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;

internal class StartSyncRunCommandValidator : AbstractValidator<StartSyncRunCommand>
{
    public StartSyncRunCommandValidator()
    {
        RuleFor(r => r.SupportedDatawalletVersion).Must(v => v > 0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code).WithMessage("'SupportedDatawalletVersion' must be greater than 0.");
    }
}
