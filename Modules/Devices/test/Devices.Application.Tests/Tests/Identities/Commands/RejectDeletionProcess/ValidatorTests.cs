using Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.RejectDeletionProcess;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new RejectDeletionProcessCommand(IdentityDeletionProcessId.Generate()));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_deletion_process_id_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new RejectDeletionProcessCommand("invalid-deletion-process-id"));

        validationResult.ShouldHaveValidationErrorFor(x => x.DeletionProcessId);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
