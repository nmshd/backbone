using Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new UpdateIdentityCommand { Address = UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress(), TierId = TierId.Generate() });

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new UpdateIdentityCommand { Address = "some-invalid-address", TierId = TierId.Generate() });

        validationResult.ShouldHaveValidationErrorFor(x => x.Address);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }

    [Fact]
    public void Fails_when_tier_id_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new UpdateIdentityCommand { Address = UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress(), TierId = "some-invalid-tier-id" });

        validationResult.ShouldHaveValidationErrorFor(x => x.TierId);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
