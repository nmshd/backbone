using GetChallengeByIdSlice = Backbone.Modules.Challenges.Module.Features.Challenges.GetChallengeById;
using Backbone.Modules.Challenges.Module.Features.Challenges.GetChallengeById;
using Backbone.Modules.Challenges.Domain.Ids;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Validator = Backbone.Modules.Challenges.Module.Features.Challenges.GetChallengeById.Validator;

namespace Backbone.Modules.Challenges.Module.Tests.Features.Challenges.GetChallengeById;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetChallengeByIdSlice.Query { Id = ChallengeId.New() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_challenge_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetChallengeByIdSlice.Query { Id = "some-invalid-challenge-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetChallengeByIdSlice.Query.Id));
    }
}
