namespace Backbone.ConsumerApi.Controllers.Files.DTOs;

public class ValidateFileTokenRequest
{
    public required string FileOwnershipToken { get; init; }
}
