namespace Backbone.ConsumerApi.Controllers.Files.DTOs;

public class CreateFileDTO
{
    public required IFormFile Content { get; set; }

    public required string OwnerSignature { get; set; }

    public required string CipherHash { get; set; }

    public required DateTime ExpiresAt { get; set; }

    public required string EncryptedProperties { get; set; }
}
