using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Xunit;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path_with_password()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipTemplatesQuery(new PaginationFilter(), new[] { new RelationshipTemplateQueryItem() { Id = RelationshipTemplateId.New(), Password = [1, 2, 3] } }));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Happy_path_without_password()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipTemplatesQuery(new PaginationFilter(), new[] { new RelationshipTemplateQueryItem() { Id = RelationshipTemplateId.New() } }));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
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
            propertyName: nameof(ListRelationshipTemplatesQuery.QueryItems),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "'Query Items' must not be empty.");
    }
}
