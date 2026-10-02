using ListRelationshipTemplatesSlice = Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Relationships.Module.Tests.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var command = new ListRelationshipTemplatesSlice.Query
        {
            PaginationFilter = new PaginationFilter(),
            Ids = [RelationshipTemplateId.New()]
        };

        var validationResult = validator.TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_Ids_is_empty()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListRelationshipTemplatesSlice.Query { PaginationFilter = new PaginationFilter(), Ids = [] });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(ListRelationshipTemplatesSlice.Query.Ids),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "'Ids' must not be empty.");
    }
}
