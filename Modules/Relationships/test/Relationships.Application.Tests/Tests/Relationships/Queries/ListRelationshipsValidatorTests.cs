using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;
using Backbone.Modules.Relationships.Domain.Ids;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Relationships.Queries;

public class ListRelationshipsValidatorTests
{
    [Fact]
    public void Happy_path()
    {
        var validator = new ListRelationshipsValidator();

        var validationResult = validator.TestValidate(new ListRelationshipsQuery(new PaginationFilter(), new[] { RelationshipId.New() }));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_Ids_is_null()
    {
        var validator = new ListRelationshipsValidator();

        var validationResult = validator.TestValidate(new ListRelationshipsQuery(new PaginationFilter(), null!));

        validationResult.ShouldHaveValidationErrorFor(q => q.Ids);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("'Ids' must not be empty.");
    }

    [Fact]
    public void Fails_when_Ids_is_empty()
    {
        var validator = new ListRelationshipsValidator();

        var validationResult = validator.TestValidate(new ListRelationshipsQuery(new PaginationFilter(), Array.Empty<RelationshipId>()));

        validationResult.ShouldHaveValidationErrorFor(q => q.Ids);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("'Ids' must not be empty.");
    }
}
