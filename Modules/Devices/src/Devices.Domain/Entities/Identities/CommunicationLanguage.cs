using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public record CommunicationLanguage
{
    public static readonly CommunicationLanguage DEFAULT_LANGUAGE = new("en");

    private static readonly string[] VALID_LANGUAGES =
    [
        "aa", "ab", "ae", "af", "ak", "am", "an", "ar", "as", "av", "ay", "az", "ba", "be", "bg", "bi", "bm", "bn", "bo", "br", "bs", "ca", "ce", "ch", "co", "cr", "cs", "cu", "cv", "cy", "da", "de",
        "dv", "dz", "ee", "el", "en", "eo", "es", "et", "eu", "fa", "ff", "fi", "fj", "fo", "fr", "fy", "ga", "gd", "gl", "gn", "gu", "gv", "ha", "he", "hi", "ho", "hr", "ht", "hu", "hy", "hz", "ia",
        "id", "ie", "ig", "ii", "ik", "io", "is", "it", "iu", "ja", "jv", "ka", "kg", "ki", "kj", "kk", "kl", "km", "kn", "ko", "kr", "ks", "ku", "kv", "kw", "ky", "la", "lb", "lg", "li", "ln", "lo",
        "lt", "lu", "lv", "mg", "mh", "mi", "mk", "ml", "mn", "mr", "ms", "mt", "my", "na", "nb", "nd", "ne", "ng", "nl", "nn", "no", "nr", "nv", "ny", "oc", "oj", "om", "or", "os", "pa", "pi", "pl",
        "ps", "pt", "qu", "rm", "rn", "ro", "ru", "rw", "sa", "sc", "sd", "se", "sg", "si", "sk", "sl", "sm", "sn", "so", "sq", "sr", "ss", "st", "su", "sv", "sw", "ta", "te", "tg", "th", "ti", "tk",
        "tl", "tn", "to", "tr", "ts", "tt", "tw", "ty", "ug", "uk", "ur", "uz", "ve", "vi", "vo", "wa", "wo", "xh", "yi", "yo", "za", "zh", "zu",
    ];

    public const int LENGTH = 2;

    private CommunicationLanguage(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<CommunicationLanguage, DomainError> Create(string value)
    {
        var validationResult = Validate(value);
        if (validationResult != null)
            return Result.Failure<CommunicationLanguage, DomainError>(validationResult);
        return new CommunicationLanguage(value);
    }

    public static DomainError? Validate(string value)
    {
        if (!VALID_LANGUAGES.Contains(value))
            return DomainErrors.InvalidDeviceCommunicationLanguage();

        return null;
    }

    public static implicit operator string(CommunicationLanguage name)
    {
        return name.Value;
    }
}
