using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Tags.Domain;

namespace Backbone.Modules.Tags.Application;

[ContainsValidTags]
public class ApplicationOptions
{
    [Required]
    public List<string> SupportedLanguages { get; set; } = [];

    public Dictionary<string, Dictionary<string, TagInfo>> TagsForAttributeValueTypes { get; set; } = [];
}

[AttributeUsage(AttributeTargets.Class)]
public class ContainsValidTagsAttribute : ValidationAttribute
{
    private static readonly string[] VALID_LANGUAGES =
    [
        "aa", "ab", "ae", "af", "ak", "am", "an", "ar", "as", "av", "ay", "az", "ba", "be", "bg", "bi", "bm", "bn", "bo", "br", "bs", "ca", "ce", "ch", "co", "cr", "cs", "cu", "cv", "cy", "da", "de",
        "dv", "dz", "ee", "el", "en", "eo", "es", "et", "eu", "fa", "ff", "fi", "fj", "fo", "fr", "fy", "ga", "gd", "gl", "gn", "gu", "gv", "ha", "he", "hi", "ho", "hr", "ht", "hu", "hy", "hz", "ia",
        "id", "ie", "ig", "ii", "ik", "io", "is", "it", "iu", "ja", "jv", "ka", "kg", "ki", "kj", "kk", "kl", "km", "kn", "ko", "kr", "ks", "ku", "kv", "kw", "ky", "la", "lb", "lg", "li", "ln", "lo",
        "lt", "lu", "lv", "mg", "mh", "mi", "mk", "ml", "mn", "mr", "ms", "mt", "my", "na", "nb", "nd", "ne", "ng", "nl", "nn", "no", "nr", "nv", "ny", "oc", "oj", "om", "or", "os", "pa", "pi", "pl",
        "ps", "pt", "qu", "rm", "rn", "ro", "ru", "rw", "sa", "sc", "sd", "se", "sg", "si", "sk", "sl", "sm", "sn", "so", "sq", "sr", "ss", "st", "su", "sv", "sw", "ta", "te", "tg", "th", "ti", "tk",
        "tl", "tn", "to", "tr", "ts", "tt", "tw", "ty", "ug", "uk", "ur", "uz", "ve", "vi", "vo", "wa", "wo", "xh", "yi", "yo", "za", "zh", "zu",
    ];

    private List<string> _supportedLanguages = [];

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not ApplicationOptions applicationOptions) return new ValidationResult($"The attribute can only be used for {nameof(ApplicationOptions)}.");

        _supportedLanguages = applicationOptions.SupportedLanguages;
        var result = ValidateLanguages();
        if (result != ValidationResult.Success) return result;

        foreach (var (attributeName, tags) in applicationOptions.TagsForAttributeValueTypes)
        {
            foreach (var (tagName, tag) in tags)
            {
                result = ValidateTag([attributeName, tagName], tag);
                if (result != ValidationResult.Success) return result;
            }
        }

        return ValidationResult.Success;
    }

    private ValidationResult? ValidateLanguages()
    {
        if (!_supportedLanguages.Contains("en")) return new ValidationResult("The tags have to support the English language.", [nameof(ApplicationOptions.SupportedLanguages)]);

        var invalidLanguageEntries = _supportedLanguages.Where(value => !VALID_LANGUAGES.Contains(value)).ToList();
        if (invalidLanguageEntries.Count != 0)
            return new ValidationResult($"The language entries \"{Enumerate(invalidLanguageEntries)}\" are not valid language codes.", [nameof(ApplicationOptions.SupportedLanguages)]);

        return ValidationResult.Success;
    }

    private ValidationResult? ValidateTag(IEnumerable<string> nameParts, TagInfo tag)
    {
        var notSupportedLanguages = tag.DisplayNames.Keys.Except(_supportedLanguages).ToList();
        var notImplementedLanguages = _supportedLanguages.Except(tag.DisplayNames.Keys).ToList();

        if (notSupportedLanguages.Count != 0)
            return new ValidationResult($"The languages \"{Enumerate(notSupportedLanguages)}\" are unsupported", [GetPathOfProperty(nameParts)]);
        if (notImplementedLanguages.Count != 0)
            return new ValidationResult($"A display name for the language(s) \"{Enumerate(notImplementedLanguages)}\" is required.", [GetPathOfProperty(nameParts)]);

        foreach (var (childName, child) in tag.Children)
        {
            var result = ValidateTag(nameParts.Append(childName), child);
            if (result != ValidationResult.Success) return result;
        }

        return ValidationResult.Success;
    }

    private static string GetPathOfProperty(IEnumerable<string> nameParts) => string.Join('.', nameParts);
    private static string Enumerate(IEnumerable<string> names) => string.Join(',', names);
}
