using System.Text.Json.Serialization;

namespace Backbone.Modules.Files.Application.Files.DTOs;

public class FileOwnershipTokenDTO
{
    [JsonConstructor]
    public FileOwnershipTokenDTO(string fileOwnershipToken)
    {
        FileOwnershipToken = fileOwnershipToken;
    }

    [JsonPropertyName("fileOwnershipToken")]
    public string FileOwnershipToken { get; set; }
}
