namespace Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

public static class GenericApplicationErrors
{
    public static ApplicationError NotFound(string recordName = "")
    {
        var formattedRecordName = string.IsNullOrEmpty(recordName) ? "Record" : recordName;

        return new ApplicationError("error.platform.recordNotFound",
            $"{formattedRecordName} not found. Make sure the ID exists and the record is not expired. If a password is required to fetch the record, make sure you passed the correct one.");
    }

    public static ApplicationError Unauthorized()
    {
        return new ApplicationError("error.platform.unauthorized",
            "You have to authenticate in order to execute this action. Further make sure that the auth token has not expired.");
    }

    public static ApplicationError Forbidden()
    {
        return new ApplicationError("error.platform.forbidden",
            "You are not allowed to perform this action due to insufficient privileges.");
    }

    public static ApplicationError QuotaExhausted()
    {
        return new ApplicationError("error.platform.quotaExhausted",
            "You cannot to perform this action because one or more quotas are exhausted.");
    }

    public static class Validation
    {
        public static ApplicationError InvalidPropertyValue(string propertyName = "")
        {
            var message = string.IsNullOrEmpty(propertyName)
                ? "A property has an invalid value."
                : $"The value of the property '{propertyName}' is invalid.";

            return new ApplicationError("error.platform.validation.invalidPropertyValue", message);
        }

        public static ApplicationError InvalidPageSize(int? maxPageSize = null)
        {
            var message = maxPageSize is null
                ? "The given page size is too high."
                : $"The given page size is too high. The maximum page size is {maxPageSize}.";

            return new ApplicationError("error.platform.validation.pagination.invalidPageSize", message);
        }

        public static ApplicationError InputCannotBeParsed(string reason = "The input cannot be parsed.")
        {
            return new ApplicationError("error.platform.inputCannotBeParsed", reason);
        }

        public static ApplicationError InvalidNumberOfRecipients(int maxNumberOfMessageRecipients)
        {
            return new ApplicationError("error.platform.validation.invalidNumberOfRecipients",
                $"The number of recipients exceeds the maximum allowed number of recipients. The maximum number of recipients is {maxNumberOfMessageRecipients}.");
        }
    }
}
