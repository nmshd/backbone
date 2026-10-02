using Microsoft.AspNetCore.Http;

namespace Backbone.Modules.Files.Module.Features.Files.CreateFile.Http;

public class RequestFormParams
{
    public required IFormFile Content { get; set; }

    public required string OwnerSignature { get; set; }

    public required string CipherHash { get; set; }

    public required DateTime ExpiresAt { get; set; }

    public required string EncryptedProperties { get; set; }
}
