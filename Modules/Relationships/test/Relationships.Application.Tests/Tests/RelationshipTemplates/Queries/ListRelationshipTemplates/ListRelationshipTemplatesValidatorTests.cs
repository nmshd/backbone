using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesValidatorTests
{
    [Fact]
    public void Happy_path()
    {
        var validator = new ListRelationshipTemplatesValidator();

        var validationResult = validator.TestValidate(new ListRelationshipTemplatesQuery(new PaginationFilter(), [RelationshipTemplateId.New()]));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_Ids_is_null()
    {
        var validator = new ListRelationshipTemplatesValidator();

        var validationResult = validator.TestValidate(new ListRelationshipTemplatesQuery(new PaginationFilter(), null));

        validationResult.ShouldHaveValidationErrorFor(q => q.Ids);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("'Ids' must not be empty.");
    }

    [Fact]
    public void Fails_when_Ids_is_empty()
    {
        var validator = new ListRelationshipTemplatesValidator();

        var validationResult = validator.TestValidate(new ListRelationshipTemplatesQuery(new PaginationFilter(), Array.Empty<RelationshipTemplateId>()));

        validationResult.ShouldHaveValidationErrorFor(q => q.Ids);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("'Ids' must not be empty.");
    }
}
