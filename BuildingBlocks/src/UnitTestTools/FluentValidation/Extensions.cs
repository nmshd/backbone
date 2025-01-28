using FluentAssertions;
using FluentValidation.TestHelper;

namespace Backbone.UnitTestTools.FluentValidation;

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
            r.ErrorCode == "error.platform.validation.invalidPropertyValue" && r.ErrorMessage.Contains("The ID or Address is not valid. Check length, prefix and the used characters."));
    }

    public static void ShouldHaveValidationErrorForIdInCollection<T>(this TestValidationResult<T> testValidationResult, string collectionWithInvalidId, int indexWithInvalidId)
    {
        var errorsForProperty = testValidationResult.ShouldHaveValidationErrorFor($"{collectionWithInvalidId}[{indexWithInvalidId}]");
        errorsForProperty.Should().Contain(r =>
            r.ErrorCode == "error.platform.validation.invalidPropertyValue" && r.ErrorMessage.Contains("The ID or Address is not valid. Check length, prefix and the used characters."));
    }
}
