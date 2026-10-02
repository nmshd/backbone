using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Module.Features.Files.GetFileContent;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using GetFileContentSlice = Backbone.Modules.Files.Module.Features.Files.GetFileContent;

namespace Backbone.Modules.Files.Module.Tests.Features.Files.GetFileContent;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetFileContentSlice.Query { Id = FileId.New() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_file_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetFileContentSlice.Query { Id = "some-invalid-file-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetFileContentSlice.Query.Id));
    }
}
