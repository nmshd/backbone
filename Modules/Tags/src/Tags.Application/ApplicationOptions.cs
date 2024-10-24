using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Tags.Domain;

namespace Backbone.Modules.Tags.Application;

public class ApplicationOptions
{
    public List<AttributeInfo> Attributes { get; set; } = [];
}

public class AttributeInfo
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public List<string> SupportedLanguages { get; set; } = [];

    public Dictionary<string, Dictionary<string, TagInfo>> TagsForAttributeValueTypes { get; set; } = [];
}

public static class DisplayNamesValidator
{
    public static ValidationResult? Validate(Dictionary<string, string> displayNames, ValidationContext context)
    {
        return displayNames.ContainsKey("en") ? ValidationResult.Success : new ValidationResult($"No English display name is provided for Tag {GetTagName(context)}");
    }

    private static string GetTagName(ValidationContext context)
    {
        var info = (TagInfo)context.ObjectInstance;
        return info.Tag;
    }
}
