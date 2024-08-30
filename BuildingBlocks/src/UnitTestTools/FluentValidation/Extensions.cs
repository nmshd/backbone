using System.Text.RegularExpressions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
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

public static class ValidationTestExtensions
{
    public static void ShouldHaveValidationErrorForItem<T>(this TestValidationResult<T> testValidationResult, string propertyName, string expectedErrorCode, string expectedErrorMessage)
    {
        var errorsForProperty = testValidationResult.ShouldHaveValidationErrorFor(propertyName);
        errorsForProperty.Should().Contain(r => r.ErrorCode == expectedErrorCode && r.ErrorMessage == expectedErrorMessage);
    }

    public static void ShouldHaveValidationErrorForItemInCollection<T>(this TestValidationResult<T> testValidationResult, string collectionWithInvalidId, int indexWithInvalidId,
        string expectedErrorCode, string expectedErrorMessage)
    {
        var errorsForProperty = testValidationResult.ShouldHaveValidationErrorFor($"{collectionWithInvalidId}[{indexWithInvalidId}]");
        errorsForProperty.Should().Contain(r => r.ErrorCode == expectedErrorCode && r.ErrorMessage == expectedErrorMessage);
    }

    public static void ShouldHaveValidationErrorForId<T>(this TestValidationResult<T> testValidationResult, string propertyWithInvalidId)
    {
        var errorsForProperty = testValidationResult.ShouldHaveValidationErrorFor(propertyWithInvalidId);
        errorsForProperty.Should().Contain(r =>
            r.ErrorCode == "error.platform.validation.invalidPropertyValue" && r.ErrorMessage == "The ID is not valid. Check length, prefix and the used characters.");
    }

    public static void ShouldHaveValidationErrorForIdInCollection<T>(this TestValidationResult<T> testValidationResult, string collectionWithInvalidId, int indexWithInvalidId)
    {
        var errorsForProperty = testValidationResult.ShouldHaveValidationErrorFor($"{collectionWithInvalidId}[{indexWithInvalidId}]");
        errorsForProperty.Should().Contain(r =>
            r.ErrorCode == "error.platform.validation.invalidPropertyValue" && r.ErrorMessage == "The ID is not valid. Check length, prefix and the used characters.");
    }
}
