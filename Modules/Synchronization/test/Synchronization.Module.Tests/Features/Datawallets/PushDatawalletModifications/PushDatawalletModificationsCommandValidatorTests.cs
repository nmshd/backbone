using Backbone.Modules.Synchronization.Module.Features.Datawallets.PushDatawalletModifications;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using FluentValidation.TestHelper;
using PushDatawalletModificationsSlice = Backbone.Modules.Synchronization.Module.Features.Datawallets.PushDatawalletModifications;

namespace Backbone.Modules.Synchronization.Module.Tests.Features.Datawallets.PushDatawalletModifications;

public class PushDatawalletModificationsCommandValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new PushDatawalletModificationsSlice.Command
        {
            Modifications =
            [
                new PushDatawalletModificationItem
                {
                    Collection = "x", DatawalletVersion = 1, EncryptedPayload = [], ObjectIdentifier = "x", PayloadCategory = "x", Type = DatawalletModificationDTO.DatawalletModificationType.Create
                }
            ],
            SupportedDatawalletVersion = 1
        });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_not_passing_a_SupportedDatawalletVersion()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new PushDatawalletModificationsSlice.Command
        {
            Modifications =
            [
                new PushDatawalletModificationItem
                {
                    Collection = "x", DatawalletVersion = 1, EncryptedPayload = [], ObjectIdentifier = "x", PayloadCategory = "x", Type = DatawalletModificationDTO.DatawalletModificationType.Create
                }
            ],
            SupportedDatawalletVersion = 0
        });

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.SupportedDatawalletVersion);
    }
}
