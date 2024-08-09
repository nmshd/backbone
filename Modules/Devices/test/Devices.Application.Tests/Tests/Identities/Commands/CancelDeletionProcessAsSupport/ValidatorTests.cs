using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcessAsSupport;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new CancelDeletionAsSupportCommand(UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress(), IdentityDeletionProcessId.Generate()));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new CancelDeletionAsSupportCommand("invalid-identity-address", IdentityDeletionProcessId.Generate()));

        validationResult.ShouldHaveValidationErrorFor(x => x.Address);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }

    [Fact]
    public void Fails_when_deletion_process_id_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new CancelDeletionAsSupportCommand(UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress(), "invalid-deletion-process-id"));

        validationResult.ShouldHaveValidationErrorFor(x => x.DeletionProcessId);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
