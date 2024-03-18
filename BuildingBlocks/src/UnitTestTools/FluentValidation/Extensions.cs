using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using Xunit.Sdk;

namespace Backbone.UnitTestTools.FluentValidation;

public static class StringExtensions
{
    public static bool MatchesRegex(this string text, string regexString)
    {
        var regex = new Regex(regexString);
        return regex.IsMatch(text);
    }
}

public static class IValidatorExtensions
{
    public static IEnumerable<ValidationFailure> ShouldHaveValidationErrorForItemWithIndex<T>(
        this IValidator<T> validator, T objectToValidate, int indexOfItem) where T : class
    {
        var validationResult = validator.Validate(objectToValidate);

        var searchedText = $"[{indexOfItem}]";

        var propertyNamesOfErrorMessages = validationResult.Errors.Select(r => r.PropertyName);

        if (!propertyNamesOfErrorMessages.Any(m => m.Contains(searchedText)))
            throw new XunitException($"Expected error for item with index {indexOfItem}.");

        return validationResult.Errors;
    }
}
