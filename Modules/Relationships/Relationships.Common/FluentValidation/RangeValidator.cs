using FluentValidation;
using FluentValidation.Validators;

namespace Relationships.Common.FluentValidation;

public class RangeValidator<T, TProperty, TRangeContent> : PropertyValidator<T, TProperty>
{
    public override string Name => "{PropertyName} is not a valid range.";

    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
        if (value == null)
            return true;

        if (value is not Range<TRangeContent> range)
            throw new InvalidOperationException($"{typeof(RangeValidator<T, TProperty, TRangeContent>).FullName} can only be applied to objects of type {typeof(Range<TRangeContent>).FullName}.");

        return range.HasFrom() || !range.HasTo();
    }
}

public static class NumberOfBytesValidatorRuleBuilderExtensions
{
    public static IRuleBuilderOptions<TObject, TProperty> IsValidRange<TObject, TProperty, TRangeContent>(this IRuleBuilder<TObject, TProperty> ruleBuilder) where TProperty : Range<TRangeContent>
    {
        return ruleBuilder
            .SetValidator(new RangeValidator<TObject, TProperty, TRangeContent>());
    }
}
