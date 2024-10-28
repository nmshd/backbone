using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
    private static readonly CultureInfo[] CULTURES = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

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

        var invalidLanguageEntries = _supportedLanguages.Where(value => CULTURES.All(c => c.TwoLetterISOLanguageName != value)).ToList();
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
