using Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.UnitTestTools.BaseClasses;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.Datawallet.Commands.PushDatawalletModifications;

public class PushDatawalletModificationsCommandValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new PushDatawalletModificationsCommand(
            [
                new PushDatawalletModificationItem
                {
                    Collection = "x", DatawalletVersion = 1, EncryptedPayload = [], ObjectIdentifier = "x", PayloadCategory = "x", Type = DatawalletModificationDTO.DatawalletModificationType.Create
                }
            ],
            1));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_not_passing_a_SupportedDatawalletVersion()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new PushDatawalletModificationsCommand(
            [
                new PushDatawalletModificationItem
                {
                    Collection = "x", DatawalletVersion = 1, EncryptedPayload = [], ObjectIdentifier = "x", PayloadCategory = "x", Type = DatawalletModificationDTO.DatawalletModificationType.Create
                }
            ],
            0));

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.SupportedDatawalletVersion);
    }
}
