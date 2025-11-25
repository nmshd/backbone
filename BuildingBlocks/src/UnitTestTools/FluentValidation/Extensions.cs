using Backbone.Tooling.Extensions;
using FluentValidation.TestHelper;
using Shouldly;

namespace Backbone.UnitTestTools.FluentValidation;

public static class ValidationTestExtensions
{
    extension<T>(TestValidationResult<T> testValidationResult)
    {
        public void ShouldHaveValidationErrorForItem(string propertyName, string expectedErrorCode, string expectedErrorMessage)
        {
            var errorsForProperty = testValidationResult.ShouldHaveValidationErrorFor(propertyName);
            errorsForProperty.ShouldContain(r => r.ErrorCode == expectedErrorCode && r.ErrorMessage.MatchesRegex(expectedErrorMessage));
        }

        public void ShouldHaveValidationErrorForItemInCollection(string collectionWithInvalidId, int indexWithInvalidId,
            string expectedErrorCode, string expectedErrorMessage)
        {
            var errorsForProperty = testValidationResult.ShouldHaveValidationErrorFor($"{collectionWithInvalidId}[{indexWithInvalidId}]");
            errorsForProperty.ShouldContain(r => r.ErrorCode == expectedErrorCode && r.ErrorMessage == expectedErrorMessage);
        }

        public void ShouldHaveValidationErrorForId(string propertyWithInvalidId)
        {
            var errorsForProperty = testValidationResult.ShouldHaveValidationErrorFor(propertyWithInvalidId);
            errorsForProperty.ShouldContain(r =>
                r.ErrorCode == "error.platform.validation.invalidPropertyValue" && r.ErrorMessage.Contains("The ID or Address is not valid. Check length, prefix and the used characters."));
        }

        public void ShouldHaveValidationErrorForIdInCollection(string collectionWithInvalidId, int indexWithInvalidId)
        {
            var errorsForProperty = testValidationResult.ShouldHaveValidationErrorFor($"{collectionWithInvalidId}[{indexWithInvalidId}]");
            errorsForProperty.ShouldContain(r =>
                r.ErrorCode == "error.platform.validation.invalidPropertyValue" && r.ErrorMessage.Contains("The ID or Address is not valid. Check length, prefix and the used characters."));
        }
    }
}
