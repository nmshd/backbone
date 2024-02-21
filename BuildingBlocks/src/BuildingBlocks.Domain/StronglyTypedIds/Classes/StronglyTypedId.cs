namespace Backbone.BuildingBlocks.Domain.StronglyTypedIds.Classes;

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

    public void Validate(string stringValue)
    {
        if (!IsValid(stringValue)) throw new InvalidIdException();
    }

    public bool IsValid(string stringValue)
    {
        var hasPrefix = stringValue.StartsWith(_prefix);
        var lengthIsValid = stringValue.Length <= _maxLength;
        var hasOnlyValidChars = stringValue.ContainsOnly(_validChars);

        return hasPrefix && lengthIsValid && hasOnlyValidChars;
    }
}

public abstract class StronglyTypedId : IFormattable, IEquatable<StronglyTypedId>, IComparable<StronglyTypedId>
{
    public const int DEFAULT_MAX_LENGTH = 20;

    public const int DEFAULT_PREFIX_LENGTH = 3;

    public const int DEFAULT_MAX_LENGTH_WITHOUT_PREFIX = DEFAULT_MAX_LENGTH - DEFAULT_PREFIX_LENGTH;

    protected static readonly char[] DEFAULT_VALID_CHARS =
    [
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        'H',
        'I',
        'J',
        'K',
        'L',
        'M',
        'N',
        'O',
        'P',
        'Q',
        'R',
        'S',
        'T',
        'U',
        'V',
        'W',
        'X',
        'Y',
        'Z',
        'a',
        'b',
        'c',
        'd',
        'e',
        'f',
        'g',
        'h',
        'i',
        'j',
        'k',
        'l',
        'm',
        'n',
        'o',
        'p',
        'q',
        'r',
        's',
        't',
        'u',
        'v',
        'w',
        'x',
        'y',
        'z',
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9'
    ];

    protected StronglyTypedId(string stringValue)
    {
        StringValue = stringValue;
    }

    public string StringValue { get; }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return StringValue;
    }

    public override string ToString()
    {
        return StringValue;
    }

    public static implicit operator string(StronglyTypedId id)
    {
        return id.StringValue;
    }

    #region Equality members

    public bool Equals(StronglyTypedId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StringValue == other.StringValue;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((StronglyTypedId)obj);
    }

    public override int GetHashCode()
    {
        return StringValue.GetHashCode();
    }

    public int CompareTo(StronglyTypedId? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(StringValue, other.StringValue, StringComparison.Ordinal);
    }

    public static bool operator ==(StronglyTypedId? left, StronglyTypedId? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(StronglyTypedId? left, StronglyTypedId? right)
    {
        return !Equals(left, right);
    }

    #endregion
}
