using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;
using Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

namespace Synchronization.Application.SyncRuns.Commands.StartSyncRun
{
    internal class StartSyncRunCommandValidator : AbstractValidator<StartSyncRunCommand>
    {
        public StartSyncRunCommandValidator()
        {
            RuleFor(r => r.SupportedDatawalletVersion).Must(v => v > 0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code).WithMessage("'SupportedDatawalletVersion' must be greater than 0.");
        }
    }
}
