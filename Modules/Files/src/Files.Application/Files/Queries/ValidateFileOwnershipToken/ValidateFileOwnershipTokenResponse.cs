namespace Backbone.Modules.Files.Application.Files.Queries.ValidateFileOwnershipToken;

public class ValidateFileOwnershipTokenResponse
{
    public ValidateFileOwnershipTokenResponse(bool isValid)
    {
        IsValid = isValid;
    }

    public bool IsValid { get; set; }
}
