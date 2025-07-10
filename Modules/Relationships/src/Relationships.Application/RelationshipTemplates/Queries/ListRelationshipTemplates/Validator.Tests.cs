using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path_with_password()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipTemplatesQuery
        {
            PaginationFilter = new PaginationFilter(),
            QueryItems = [new ListRelationshipTemplatesQueryItem { Id = RelationshipTemplateId.New(), Password = [1, 2, 3] }]
        });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Happy_path_without_password()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var command = new ListRelationshipTemplatesQuery
        {
            PaginationFilter = new PaginationFilter(),
            QueryItems = [new ListRelationshipTemplatesQueryItem { Id = RelationshipTemplateId.New() }]
        };

        var validationResult = validator.TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_Queries_is_empty()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipTemplatesQuery { PaginationFilter = new PaginationFilter(), QueryItems = [] });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(ListRelationshipTemplatesQuery.QueryItems),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "'Query Items' must not be empty.");
    }
}
