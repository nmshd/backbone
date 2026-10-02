using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.StartSyncRun;
using FluentValidation.TestHelper;
using StartSyncRunSlice = Backbone.Modules.Synchronization.Module.Features.SyncRuns.StartSyncRun;

namespace Backbone.Modules.Synchronization.Module.Tests.Features.SyncRuns.StartSyncRun;

public class StartSyncRunCommandValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new StartSyncRunSlice.Command { Type = SyncRunDTO.SyncRunType.DatawalletVersionUpgrade, SupportedDatawalletVersion = 1 });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_not_passing_a_SupportedDatawalletVersion()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new StartSyncRunSlice.Command { Type = SyncRunDTO.SyncRunType.DatawalletVersionUpgrade, SupportedDatawalletVersion = 0 });

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.SupportedDatawalletVersion);
    }
}
