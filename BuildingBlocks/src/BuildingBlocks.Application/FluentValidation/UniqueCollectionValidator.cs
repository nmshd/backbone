using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;
using FluentValidation.Validators;

namespace Backbone.BuildingBlocks.Application.FluentValidation;

public class UniqueCollectionValidator<T, TElement, TPropertyKey> : PropertyValidator<T, IEnumerable<TElement>>
{
    private readonly Func<TElement, TPropertyKey> _getUniqueValue;

    public UniqueCollectionValidator(Func<TElement, TPropertyKey> getUniqueValue)
    {
        _getUniqueValue = getUniqueValue;
    }

    public override string Name => "UniqueCollectionValidator";

    public override bool IsValid(ValidationContext<T> context, IEnumerable<TElement>? value)
    {
        if (value == null) return true;

        if (value is not { } enumerable)
            throw new ArgumentException("PropertyValue must implement IEnumerable<T>.");

        var uniqueItemValues = new HashSet<object?>();

        foreach (var item in enumerable)
        {
            if (item == null) continue;

            var itemValue = _getUniqueValue(item);
            if (uniqueItemValues.Any(i => Equals(i, itemValue))) return false;

            uniqueItemValues.Add(itemValue);
        }

        return true;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "The same item cannot be added more than once.";
    }
}

public static class UniqueCollectionValidatorRuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, IEnumerable<TElement>> UniqueItems<T, TElement, TPropertyKey>(
        this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder, Func<TElement, TPropertyKey> uniqueConstraint)
    {
        return ruleBuilder
            .SetValidator(new UniqueCollectionValidator<T, TElement, TPropertyKey>(uniqueConstraint))
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
