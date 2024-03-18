using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.SyncRuns.Commands.StartSyncRun;

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
