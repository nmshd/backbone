using Backbone.Modules.Challenges.Application.Challenges.Queries.GetChallengeById;
using Backbone.Modules.Challenges.Domain.Ids;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;
using Validator = Backbone.Modules.Challenges.Application.Challenges.Queries.GetChallengeById.Validator;

namespace Backbone.Modules.Challenges.Application.Tests.Tests.Challenges.Queries.GetChallengeById;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new GetChallengeByIdQuery { Id = ChallengeId.New() });

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_challenge_id_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new GetChallengeByIdQuery { Id = "some-invalid-challenge-id" });

        validationResult.ShouldHaveValidationErrorFor(x => x.Id);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
