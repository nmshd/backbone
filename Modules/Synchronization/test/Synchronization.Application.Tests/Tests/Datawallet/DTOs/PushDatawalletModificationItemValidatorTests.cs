using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.Datawallet.DTOs;

public class PushDatawalletModificationItemValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Accepts_a_valid_object()
    {
        // Arrange
        var validator = new PushDatawalletModificationItemValidator();

        // Act
        var validationResult = validator.TestValidate(CreateCommand());

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Accepts_when_payload_category_is_null()
    {
        // Arrange
        var validator = new PushDatawalletModificationItemValidator();

        // Act
        var validationResult = validator.TestValidate(CreateCommand(payloadCategory: null));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Expects_payload_category_to_not_have_more_than_50_characters()
    {
        // Arrange
        var validator = new PushDatawalletModificationItemValidator();

        // Act
        var validationResult = validator.TestValidate(CreateCommand(payloadCategory: CreateRandomString(51)));

        // Assert
        validationResult.ShouldHaveValidationErrorFor("PayloadCategory").WithErrorMessage("The length of 'Payload Category' must be 50 characters or fewer. You entered 51 characters.");
    }

    [Fact]
    public void Expects_collection_to_not_have_more_than_50_characters()
    {
        // Arrange
        var validator = new PushDatawalletModificationItemValidator();

        // Act
        var validationResult = validator.TestValidate(CreateCommand(collection: CreateRandomString(51)));

        // Assert
        validationResult.ShouldHaveValidationErrorFor("Collection").WithErrorMessage("The length of 'Collection' must be 50 characters or fewer. You entered 51 characters.");
    }

    [Fact]
    public void Expects_object_identifier_to_not_have_more_than_100_characters()
    {
        // Arrange
        var validator = new PushDatawalletModificationItemValidator();

        // Act
        var validationResult = validator.TestValidate(CreateCommand(objectIdentifier: CreateRandomString(101)));

        // Assert
        validationResult.ShouldHaveValidationErrorFor("ObjectIdentifier").WithErrorMessage("The length of 'Object Identifier' must be 100 characters or fewer. You entered 101 characters.");
    }

    private static PushDatawalletModificationItem CreateCommand(string objectIdentifier = "testObjectIdentifier", string collection = "testCollection", string? payloadCategory = "testPayloadCategory")
    {
        var command = new PushDatawalletModificationItem
        {
            Collection = collection,
            DatawalletVersion = 1,
            ObjectIdentifier = objectIdentifier,
            Type = DatawalletModificationDTO.DatawalletModificationType.Create,
            EncryptedPayload = [0],
            PayloadCategory = payloadCategory,
        };

        return command;
    }
}
