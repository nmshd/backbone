using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

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
            return GenericDomainErrors.InvalidIdPrefix($"Id starts with {new string(stringValue.Take(3).ToArray())} instead of {_prefix}");

        var lengthIsValid = stringValue.Length == _maxLength;
        if (!lengthIsValid)
            return GenericDomainErrors.InvalidIdLength($"Id has a length of {stringValue.Length} instead of {_maxLength}");

        var hasOnlyValidChars = stringValue.ContainsOnly(_validChars);
        if (!hasOnlyValidChars)
            return GenericDomainErrors.InvalidIdCharacters($"Valid characters are {_validChars}");

        return null;
    }
}
