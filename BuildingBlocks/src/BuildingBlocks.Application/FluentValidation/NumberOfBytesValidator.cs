using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;
using FluentValidation.Validators;

namespace Backbone.BuildingBlocks.Application.FluentValidation;

public class NumberOfBytesValidator<T> : PropertyValidator<T, byte[]?>, ILengthValidator
{
    public NumberOfBytesValidator(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public NumberOfBytesValidator(int numberOfBytes)
    {
        Min = numberOfBytes;
        Max = numberOfBytes;
    }

    public int Max { get; set; }

    public int Min { get; set; }

    public override string Name => "NumberOfBytesValidator";

    public override bool IsValid(ValidationContext<T> context, byte[]? value)
    {
        var numberOfBytes = value?.Length ?? 0;

        if (value == null && Min != 0 // null is allowed as long as an empty array is allowed
            || numberOfBytes < Min
            || numberOfBytes > Max)
        {
            context.MessageFormatter.AppendArgument("Min", Min);
            context.MessageFormatter.AppendArgument("Max", Max);
            context.MessageFormatter.AppendArgument("Actual", numberOfBytes);
            return false;
        }

        return true;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return Min == Max
            ? "'{PropertyName}' must be {Min} bytes long. You entered {Actual} bytes."
            : "'{PropertyName}' must be between {Min} and {Max} bytes long. You entered {Actual} bytes.";
    }
}

public static class NumberOfBytesValidatorRuleBuilderExtensions
{
    extension<T>(IRuleBuilder<T, byte[]?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, byte[]?> NumberOfBytes(int minNumberOfBytes, int maxNumberOfBytes)
        {
            return ruleBuilder
                .SetValidator(new NumberOfBytesValidator<T>(minNumberOfBytes, maxNumberOfBytes))
                .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        }

        public IRuleBuilderOptions<T, byte[]?> NumberOfBytes(int numberOfBytes)
        {
            return ruleBuilder
                .SetValidator(new NumberOfBytesValidator<T>(numberOfBytes))
                .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        }
    }
}
