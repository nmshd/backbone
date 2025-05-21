using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.RegenerateFileOwnershipToken;

public class RegenerateFileOwnershipTokenResponse
{
    public RegenerateFileOwnershipTokenResponse(File file)
    {
        NewOwnershipToken = file.OwnershipToken.Value;
    }

    public string NewOwnershipToken { get; set; }
}
