using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Module.Features.Files.ClaimFileOwnership;

public class ClaimFileOwnershipResponse
{
    public ClaimFileOwnershipResponse(File file)
    {
        NewOwnershipToken = file.OwnershipToken.Value;
    }

    public string NewOwnershipToken { get; init; }
}
