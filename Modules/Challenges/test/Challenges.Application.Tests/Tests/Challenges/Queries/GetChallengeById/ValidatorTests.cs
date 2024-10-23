using Backbone.Modules.Challenges.Application.Challenges.Queries.GetChallengeById;
using Backbone.Modules.Challenges.Domain.Ids;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Validator = Backbone.Modules.Challenges.Application.Challenges.Queries.GetChallengeById.Validator;

namespace Backbone.Modules.Challenges.Application.Tests.Tests.Challenges.Queries.GetChallengeById;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetChallengeByIdQuery { Id = ChallengeId.New() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_challenge_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetChallengeByIdQuery { Id = "some-invalid-challenge-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetChallengeByIdQuery.Id));
    }
}
