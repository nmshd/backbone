using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Backbone.UnitTestTools.BaseClasses;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.SyncRuns.Commands.StartSyncRun;

public class StartSyncRunCommandValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new StartSyncRunCommand(SyncRunDTO.SyncRunType.DatawalletVersionUpgrade, 1));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_not_passing_a_SupportedDatawalletVersion()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new StartSyncRunCommand(SyncRunDTO.SyncRunType.DatawalletVersionUpgrade, 0));

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.SupportedDatawalletVersion);
    }
}
