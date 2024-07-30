using Backbone.Modules.Devices.Application.Identities.Commands.LogDeletionProcess;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.LogDeletionProcess;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new LogDeletionProcessCommand(UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress(), "aggregateType"));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new LogDeletionProcessCommand("invalid-identity-address", "aggregateType"));

        validationResult.ShouldHaveValidationErrorFor(x => x.IdentityAddress);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
