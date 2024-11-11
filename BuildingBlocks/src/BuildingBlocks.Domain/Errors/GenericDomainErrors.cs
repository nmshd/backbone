namespace Backbone.BuildingBlocks.Domain.Errors;

public static class GenericDomainErrors
{
    public static DomainError NotFound(string? recordName = null)
    {
        return new DomainError("error.platform.recordNotFound", $"{recordName} not found");
    }

    public static DomainError InvalidIdPrefix(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.invalidId",
            string.IsNullOrEmpty(reason) ? $"The ID's prefix is invalid {formattedReason}." : reason);
    }

    public static DomainError InvalidIdLength(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.invalidId",
            string.IsNullOrEmpty(reason) ? $"The ID's length is invalid {formattedReason}." : reason);
    }

    public static DomainError InvalidIdCharacters(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.invalidId",
            string.IsNullOrEmpty(reason) ? $"The ID contains invalid characters {formattedReason}." : reason);
    }

    public static DomainError NewAndOldParametersMatch(string nameOfParameter)
    {
        return new DomainError("error.platform.validation.newAndOldMatch",
            $"The new {nameOfParameter} and the old {nameOfParameter} cannot be the same.");
    }

    public static DomainError Forbidden()
    {
        return new DomainError("error.platform.forbidden",
            "You are not allowed to perform this action due to insufficient privileges.");
    }
}
