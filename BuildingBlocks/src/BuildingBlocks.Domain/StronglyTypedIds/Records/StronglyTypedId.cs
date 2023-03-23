namespace Enmeshed.BuildingBlocks.Domain.StronglyTypedIds.Records;

public abstract record StronglyTypedId
{
    public const int DEFAULT_MAX_LENGTH = 20;

    public const int DEFAULT_PREFIX_LENGTH = 3;

    public const int DEFAULT_MAX_LENGTH_WITHOUT_PREFIX = DEFAULT_MAX_LENGTH - DEFAULT_PREFIX_LENGTH;

    protected static readonly char[] DefaultValidChars =
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
        'V', 'W', 'X', 'Y', 'Z',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
        'v', 'w', 'x', 'y', 'z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    public string Value { get; }

    protected StronglyTypedId(string value)
    {
        Value = value;
    }
}
