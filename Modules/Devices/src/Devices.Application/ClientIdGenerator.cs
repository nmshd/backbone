using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Devices.Application;

public static class ClientIdGenerator
{
    public const int MAX_LENGTH = 20;
    public const int PREFIX_LENGTH = 3;
    public const int MAX_LENGTH_WITHOUT_PREFIX = MAX_LENGTH - PREFIX_LENGTH;
    public const string PREFIX = "CLT";

    private static readonly char[] VALID_CHARS =
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

    public static string Generate()
    {
        var stringValue = StringUtils.Generate(VALID_CHARS, MAX_LENGTH_WITHOUT_PREFIX);
        return PREFIX + stringValue;
    }
}
