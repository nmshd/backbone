using Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengesOfIdentity;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Challenges.Application.Tests.Tests.Challenges.Commands.DeleteChallengesOfIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new DeleteChallengesOfIdentityCommand(TestDataGenerator.CreateRandomIdentityAddress()));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new DeleteChallengesOfIdentityCommand("invalid-identity-address"));

        validationResult.ShouldHaveValidationErrorFor(x => x.IdentityAddress);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
