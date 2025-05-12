namespace Backbone.Modules.Files.Application.Files.Commands.RegenerateFileOwnershipToken;

public class RegenerateFileOwnershipTokenResponse
{
    public string NewOwnershipToken { get; set; }

    public RegenerateFileOwnershipTokenResponse(string newOwnershipToken)
    {
        NewOwnershipToken = newOwnershipToken;
    }
}
