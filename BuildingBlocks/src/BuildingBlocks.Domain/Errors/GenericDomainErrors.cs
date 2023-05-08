namespace Enmeshed.BuildingBlocks.Domain.Errors;

public static class GenericDomainErrors
{
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
}
