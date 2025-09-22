namespace Backbone.ConsumerApi.Controllers.Files.DTOs;

public class ValidateFileOwnershipTokenRequest
{
    public required string OwnershipToken { get; init; }
}
