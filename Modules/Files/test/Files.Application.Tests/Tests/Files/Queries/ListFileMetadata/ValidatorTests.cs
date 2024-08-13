using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Files.Application.Files.Queries.ListFileMetadata;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Files.Application.Tests.Tests.Files.Queries.ListFileMetadata;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListFileMetadataQuery(new PaginationFilter(), [FileId.New()]));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_file_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListFileMetadataQuery(new PaginationFilter(), ["some-invalid-file-id"]));

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(ListFileMetadataQuery.Ids),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "The ID is not valid. Check length, prefix and the used characters.");
    }
}
