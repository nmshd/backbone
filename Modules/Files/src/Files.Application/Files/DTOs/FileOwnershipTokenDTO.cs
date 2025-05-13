using System.Text.Json.Serialization;

namespace Backbone.Modules.Files.Application.Files.DTOs;

public class FileOwnershipTokenDTO
{
    [JsonConstructor]
    public FileOwnershipTokenDTO(string value)
    {
        Value = value;
    }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}
