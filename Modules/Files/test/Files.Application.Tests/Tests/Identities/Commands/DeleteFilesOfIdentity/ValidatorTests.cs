using Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Files.Application.Tests.Tests.Identities.Commands.DeleteFilesOfIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new DeleteFilesOfIdentityCommand(TestDataGenerator.CreateRandomIdentityAddress()));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new DeleteFilesOfIdentityCommand("invalid-identity-address"));

        validationResult.ShouldHaveValidationErrorFor(x => x.IdentityAddress);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
