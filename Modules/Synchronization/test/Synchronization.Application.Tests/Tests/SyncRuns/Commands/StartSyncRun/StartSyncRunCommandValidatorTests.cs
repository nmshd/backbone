using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.UnitTestTools.BaseClasses;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.SyncRuns.Commands.StartSyncRun;

public class StartSyncRunCommandValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var command = new StartSyncRunCommand(SyncRun.SyncRunType.DatawalletVersionUpgrade, 1);
        var validationResult = validator.TestValidate(command);

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_not_passing_a_SupportedDatawalletVersion()
    {
        var validator = new Validator();

        var command = new StartSyncRunCommand(SyncRun.SyncRunType.DatawalletVersionUpgrade, 0);
        var validationResult = validator.TestValidate(command);

        validationResult.ShouldHaveValidationErrorFor(x => x.SupportedDatawalletVersion);
    }
}
