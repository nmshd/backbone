using Backbone.BuildingBlocks.Application.Extensions;
using FluentValidation;

namespace Backbone.BuildingBlocks.Application.Tests.FluentValidation;

public class ValueInValidatorTests : AbstractTestsBase
{
    [Theory]
    [InlineData("validValue1")]
    [InlineData("validValue2")]
    public void ValidationPassesForCorrectValue(string value)
    {
        var validator = new AClassValidator();

        var validationResult = validator.Validate(new AClass { AStringProperty = value });

        validationResult.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void ValidationFailsForInvalidValue()
    {
        var validator = new AClassValidator();

        var validationResult = validator.Validate(new AClass { AStringProperty = "invalidValue" });

        validationResult.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void ValidationErrorHasCorrectProperties()
    {
        var validator = new AClassValidator();

        var validationResult = validator.Validate(new AClass { AStringProperty = "invalidValue" });

        var validationError = validationResult.Errors[0];

        validationError.PropertyName.ShouldBe(nameof(AClass.AStringProperty));
        validationError.ErrorCode.ShouldBe("error.platform.validation.invalidPropertyValue");
        validationError.ErrorMessage.ShouldSatisfyAllConditions(
            m => m.ShouldContain("'A String Property'"),
            m => m.ShouldContain("validValue1, validValue2")
        );
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
