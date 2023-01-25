using FluentValidation.TestHelper;
using Synchronization.Application.SyncRuns.Commands.StartSyncRun;
using Synchronization.Application.SyncRuns.DTOs;
using Xunit;

namespace Synchronization.Application.Tests.Tests.SyncRuns.Commands.StartSyncRun
{
    public class StartSyncRunCommandValidatorTests
    {
        [Fact]
        public void Happy_path()
        {
            var validator = new StartSyncRunCommandValidator();

            var command = new StartSyncRunCommand(SyncRunDTO.SyncRunType.DatawalletVersionUpgrade, 1);
            var validationResult = validator.TestValidate(command);

            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Fails_when_not_passing_a_SupportedDatawalletVersion()
        {
            var validator = new StartSyncRunCommandValidator();

            var command = new StartSyncRunCommand(SyncRunDTO.SyncRunType.DatawalletVersionUpgrade, 0);
            var validationResult = validator.TestValidate(command);

            validationResult.ShouldHaveValidationErrorFor(x => x.SupportedDatawalletVersion);
        }
    }
}
