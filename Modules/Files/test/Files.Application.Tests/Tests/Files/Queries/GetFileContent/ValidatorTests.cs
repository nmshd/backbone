using Backbone.Modules.Files.Application.Files.Queries.GetFileContent;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Files.Application.Tests.Tests.Files.Queries.GetFileContent;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetFileContentQuery { Id = FileId.New() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_file_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetFileContentQuery { Id = "some-invalid-file-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(
            propertyWithInvalidId: nameof(GetFileContentQuery.Id));
    }
}
