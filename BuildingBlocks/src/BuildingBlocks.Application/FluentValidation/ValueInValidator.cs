using System.Collections;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using FluentValidation;
using FluentValidation.Validators;

namespace Backbone.BuildingBlocks.Application.FluentValidation;

public class ValueInValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
    private readonly string _commaSeparatedListOfValidValues;

    public ValueInValidator(IEnumerable validValues)
    {
        if (validValues == null) throw new ArgumentNullException(nameof(validValues));

        ValidValues = validValues.Cast<object?>().ToList();

        var numberOfValidValues = ValidValues.Count();

        if (numberOfValidValues == 0)
            throw new ArgumentException("At least one valid option is expected", nameof(validValues));

        _commaSeparatedListOfValidValues = $"[{string.Join(", ", ValidValues.Select(v => v == null ? "null" : v.ToString()))}]";
    }

    public IEnumerable<object?> ValidValues { get; }

    public override string Name => "ValueInValidator";

    private void ValidateValidValuesType()
    {
        var propertyValueType = typeof(TProperty);

        if (ValidValues.Any(validValue => validValue == null || validValue.GetType() != propertyValueType))
        {
            throw new ArgumentException(
                $"All objects in 'validValues' have to be of type '{propertyValueType}'");
        }
    }

    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
        if (value == null) return ValidValues.Contains(null);

        ValidateValidValuesType();

        if (ValidValues.Contains(value)) return true;

        context.MessageFormatter.AppendArgument("ValidValues", _commaSeparatedListOfValidValues);

        return false;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "'{PropertyName}' must have one of the following values: {ValidValues}";
    }
}

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, TProperty> In<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
        params TProperty[] validOptions)
    {
        return ruleBuilder
            .SetValidator(new ValueInValidator<T, TProperty>(validOptions))
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }

    public static IRuleBuilderOptions<T, string?> ValidId<T, TId>(this IRuleBuilder<T, string?> ruleBuilder) where TId : StronglyTypedId
    {
        return ruleBuilder
            .Must(x => (bool)typeof(TId).GetMethod("IsValid")!.Invoke(null, [x])!)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("The ID is not valid. Check length, prefix and the used characters.");
    }
}
