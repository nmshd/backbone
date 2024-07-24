using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Queries;

public class ListRelationshipsValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new ListRelationshipsQuery(new PaginationFilter(), [RelationshipId.New().Value]));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_Ids_is_null()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new ListRelationshipsQuery(new PaginationFilter(), null));

        validationResult.ShouldHaveValidationErrorFor(q => q.Ids);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("'Ids' must not be empty.");
    }

    [Fact]
    public void Fails_when_Ids_is_empty()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new ListRelationshipsQuery(new PaginationFilter(), []));

        validationResult.ShouldHaveValidationErrorFor(q => q.Ids);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("'Ids' must not be empty.");
    }
}
