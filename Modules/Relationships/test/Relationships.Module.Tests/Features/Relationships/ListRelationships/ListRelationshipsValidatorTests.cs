using ListRelationshipsSlice = Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationships;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationships;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Relationships.Module.Tests.Features.Relationships.ListRelationships;

public class ListRelationshipsValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipsSlice.Query { PaginationFilter = new PaginationFilter(), Ids = [RelationshipId.New().Value] });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_Ids_is_null()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipsSlice.Query { PaginationFilter = new PaginationFilter(), Ids = [] });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(ListRelationshipsSlice.Query.Ids),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "'Ids' must not be empty.");
    }

    [Fact]
    public void Fails_when_Ids_is_empty()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipsSlice.Query { PaginationFilter = new PaginationFilter(), Ids = [] });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(ListRelationshipsSlice.Query.Ids),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "'Ids' must not be empty.");
    }
}
