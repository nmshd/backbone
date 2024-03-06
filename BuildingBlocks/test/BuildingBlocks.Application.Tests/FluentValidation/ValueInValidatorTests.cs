using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace Backbone.BuildingBlocks.Application.Tests.FluentValidation;

public class ValueInValidatorTests
{
    [Theory]
    [InlineData("validValue1")]
    [InlineData("validValue2")]
    public void ValidationPassesForCorrectValue(string value)
    {
        var validator = new AClassValidator();

        var validationResult = validator.Validate(new AClass { AStringProperty = value });

        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidationFailsForInvalidValue()
    {
        var validator = new AClassValidator();

        var validationResult = validator.Validate(new AClass { AStringProperty = "invalidValue" });

        validationResult.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidationErrorHasCorrectProperties()
    {
        var validator = new AClassValidator();

        var validationResult = validator.Validate(new AClass { AStringProperty = "invalidValue" });

        var validationError = validationResult.Errors[0];

        validationError.PropertyName.Should().Be(nameof(AClass.AStringProperty));
        validationError.ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationError.ErrorMessage
            .Should().Contain("'A String Property'").And.Subject
            .Should().Contain("validValue1, validValue2");
    }

    private class AClass
    {
        public required string AStringProperty { get; init; }
    }

    private class AClassValidator : AbstractValidator<AClass>
    {
        public AClassValidator()
        {
            RuleFor(c => c.AStringProperty).In("validValue1", "validValue2");
        }
    }
}
