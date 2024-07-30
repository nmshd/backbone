using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.ListIdentities;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new ListIdentitiesQuery([UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress()], IdentityStatus.Active));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new ListIdentitiesQuery(["some-invalid-address"], IdentityStatus.Active));

        validationResult.ShouldHaveValidationErrorFor(x => x.Addresses);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
