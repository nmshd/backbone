namespace Backbone.Modules.Files.Application.Files.Commands.ClaimFileOwnership;

public class ClaimFileOwnershipResponse
{
    public string NewOwnershipToken { get; set; }

    public ClaimFileOwnershipResponse(string newOwnershipToken)
    {
        NewOwnershipToken = newOwnershipToken;
    }
}
