using System.ComponentModel.DataAnnotations;

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
    [MinLength(1)]
    public List<TagInfo> Tags { get; set; } = [];
}

public class TagInfo
{
    [Required]
    [MinLength(1)]
    public string Tag { get; set; } = string.Empty;

    [Required]
    [CustomValidation(typeof(DisplayNamesValidator), "Validate")]
    public Dictionary<string, string> DisplayNames { get; set; } = [];

    public List<TagInfo> Children { get; set; } = [];
}

public static class DisplayNamesValidator
{
    public static ValidationResult? Validate(Dictionary<string, string> displayNames, ValidationContext context)
    {
        return displayNames.ContainsKey("en") ? ValidationResult.Success : new ValidationResult($"No english display name is provided for Tag {GetTagName(context)}");
    }

    private static string GetTagName(ValidationContext context)
    {
        var info = (TagInfo)context.ObjectInstance;
        return info.Tag;
    }
}
