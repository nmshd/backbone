using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Tags.Domain;

public class TagInfo
{
    [Required]
    public Dictionary<string, string> DisplayNames { get; set; } = [];

    public Dictionary<string, TagInfo> Children { get; set; } = [];
}
