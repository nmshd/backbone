using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.DTOs;

public class FileOwnershipTokenDTO
{
    public FileOwnershipTokenDTO(File file)
    {
        TokenContent = file.OwnershipToken.Value;
    }

    public string TokenContent { get; set; }
}
