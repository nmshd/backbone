using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Module.Features.Files.ListFileMetadata;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using ListFileMetadataSlice = Backbone.Modules.Files.Module.Features.Files.ListFileMetadata;

namespace Backbone.Modules.Files.Module.Tests.Features.Files.ListFileMetadata;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListFileMetadataSlice.Query { PaginationFilter = new PaginationFilter(), Ids = [FileId.New()] });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_file_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListFileMetadataSlice.Query { PaginationFilter = new PaginationFilter(), Ids = ["some-invalid-file-id"] });

        // Assert
        validationResult.ShouldHaveValidationErrorForIdInCollection(
            collectionWithInvalidId: nameof(ListFileMetadataSlice.Query.Ids),
            indexWithInvalidId: 0);
    }
}
