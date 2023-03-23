using Enmeshed.BuildingBlocks.Domain.Errors;

namespace BuildingBlocks.Domain.Exceptions;

public static class GenericDomainErrors
{
    public static DomainError InvalidIdPrefix(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.domain.invalidIdPrefix",
            string.IsNullOrEmpty(reason) ? $"The ID's prefix is invalid {formattedReason}." : reason);
    }

    public static DomainError InvalidIdLenght(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.domain.invalidIdLenght",
            string.IsNullOrEmpty(reason) ? $"The ID's length is invalid {formattedReason}." : reason);
    }

    public static DomainError InvalidIdCharacters(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.domain.invalidIdCharacters",
            string.IsNullOrEmpty(reason) ? $"The ID contains invalid characters {formattedReason}." : reason);
    }
}
