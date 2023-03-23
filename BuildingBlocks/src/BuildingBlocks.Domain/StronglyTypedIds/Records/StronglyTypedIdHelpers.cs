using BuildingBlocks.Domain.Exceptions;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Enmeshed.BuildingBlocks.Domain.StronglyTypedIds.Records;

public class StronglyTypedIdHelpers
{
    private readonly int _maxLength;
    private readonly string _prefix;
    private readonly char[] _validChars;

    public StronglyTypedIdHelpers(string prefix, char[] validChars, int maxLength)
    {
        _prefix = prefix;
        _validChars = validChars;
        _maxLength = maxLength;
    }

    public DomainError? Validate(string stringValue)
    {
        var hasPrefix = stringValue.StartsWith(_prefix);
        if (!hasPrefix)
            return GenericDomainErrors.InvalidIdPrefix($"Id starts with {stringValue.Take(3)} instead of {_prefix}");

        var lengthIsValid = stringValue.Length <= _maxLength;
        if (!lengthIsValid)
            return GenericDomainErrors.InvalidIdLength($"Id has a length of {stringValue.Length} while the max is {_maxLength}");

        var hasOnlyValidChars = stringValue.ContainsOnly(_validChars);
        if (!hasOnlyValidChars)
            return GenericDomainErrors.InvalidIdCharacters($"Valid characters are {_validChars}");

        return null;
    }
}
