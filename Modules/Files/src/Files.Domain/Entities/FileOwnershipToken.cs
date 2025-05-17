using System.ComponentModel;
using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Files.Domain.Entities;

[Serializable]
[TypeConverter(typeof(FileOwnershipToken))]
public record FileOwnershipToken(string Value)
{
    public static readonly int DEFAULT_MAX_LENGTH = 20;

    protected static readonly char[] ALLOWED_CHARACTERS =
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
        '9',
        '!',
        '@',
        '#',
        '$',
        '%',
        '^',
        '&',
        '*',
        '(',
        ')',
        '_',
        '-',
        '+',
        '=',
        '<',
        '>',
        '?',
        '/',
        '{',
        '}',
        '[',
        ']',
        '|',
        '~'
    ];

    public static FileOwnershipToken New()
    {
        var stringValue = StringUtils.Generate(ALLOWED_CHARACTERS, DEFAULT_MAX_LENGTH);
        return new FileOwnershipToken(stringValue);
    }

    public static FileOwnershipToken Parse(string value)
    {
        return new FileOwnershipToken(value);
    }

    public static bool IsValid(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        if (value.Length > DEFAULT_MAX_LENGTH)
            return false;

        return value.All(c => ALLOWED_CHARACTERS.Contains(c));
    }
}
