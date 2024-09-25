using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipTemplatesQuery(new PaginationFilter(), new[] { new RelationshipTemplateQuery() { Id = RelationshipTemplateId.New() } }));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_Queries_is_null()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipTemplatesQuery(new PaginationFilter(), null));

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(ListRelationshipTemplatesQuery.Queries),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "'Queries' must not be empty.");
    }

    [Fact]
    public void Fails_when_Queries_is_empty()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipTemplatesQuery(new PaginationFilter(), []));

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(ListRelationshipTemplatesQuery.Queries),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "'Queries' must not be empty.");
    }
}
