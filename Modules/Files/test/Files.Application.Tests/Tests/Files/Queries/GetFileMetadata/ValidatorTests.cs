using Backbone.Modules.Files.Application.Files.Queries.GetFileMetadata;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Files.Application.Tests.Tests.Files.Queries.GetFileMetadata;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetFileMetadataQuery { Id = FileId.New() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_file_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetFileMetadataQuery { Id = "some-invalid-file-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetFileMetadataQuery.Id));
    }
}
