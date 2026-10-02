using GetFileMetadataSlice = Backbone.Modules.Files.Module.Features.Files.GetFileMetadata;
using Backbone.Modules.Files.Module.Features.Files.GetFileMetadata;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Files.Module.Tests.Tests.Files.Queries.GetFileMetadata;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetFileMetadataSlice.Query { Id = FileId.New() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_file_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetFileMetadataSlice.Query { Id = "some-invalid-file-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetFileMetadataSlice.Query.Id));
    }
}
