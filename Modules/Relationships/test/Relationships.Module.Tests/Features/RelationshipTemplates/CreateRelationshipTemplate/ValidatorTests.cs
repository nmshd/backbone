using CreateRelationshipTemplateSlice = Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.CreateRelationshipTemplate;
using Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.CreateRelationshipTemplate;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Relationships.Module.Tests.Features.RelationshipTemplates.CreateRelationshipTemplate;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_Path_with_optional_parameters()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateRelationshipTemplateSlice.Command
            {
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                MaxNumberOfAllocations = 1,
                Content = [1],
                ForIdentity = CreateRandomIdentityAddress(), Password = [1]
            });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Happy_Path_without_optional_parameters()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateRelationshipTemplateSlice.Command
            {
                Content = [1]
            });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_ExpiresAt_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateRelationshipTemplateSlice.Command
            {
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                Content = [1]
            });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(nameof(CreateRelationshipTemplateSlice.Command.ExpiresAt), "error.platform.validation.invalidPropertyValue", "'Expires At' must be in the future.");
    }

    [Fact]
    public void Fails_when_MaxNumberOfAllocations_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateRelationshipTemplateSlice.Command
            {
                MaxNumberOfAllocations = 0,
                Content = [1]
            });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(nameof(CreateRelationshipTemplateSlice.Command.MaxNumberOfAllocations), "error.platform.validation.invalidPropertyValue",
            "'Max Number Of Allocations' must be greater than '0'.");
    }

    [Fact]
    public void Fails_when_ForIdentity_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateRelationshipTemplateSlice.Command
            {
                Content = [1],
                ForIdentity = "some-address"
            });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(CreateRelationshipTemplateSlice.Command.ForIdentity));
    }

    [Fact]
    public void Fails_when_Password_is_too_long()
    {
        // Arrange
        var validator = new Validator();

        var password = new byte[250];
        new Random().NextBytes(password);

        // Act
        var validationResult = validator.TestValidate(
            new CreateRelationshipTemplateSlice.Command
            {
                Content = [1],
                Password = password
            });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(nameof(CreateRelationshipTemplateSlice.Command.Password), "error.platform.validation.invalidPropertyValue",
            "'Password' must be between 1 and 200 bytes long. You entered 250 bytes.");
    }
}
