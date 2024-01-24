using Backbone.DevelopmentKit.Identity.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Backbone.Modules.Files.ConsumerApi.DTOs;

public class CreateFileDTO
{
    public IFormFile Content { get; set; }
    public IdentityAddress? Owner { get; set; }
    public required byte[] OwnerSignature { get; set; }

    public required byte[] CipherHash { get; set; }

    public DateTime ExpiresAt { get; set; }

    public required byte[] EncryptedProperties { get; set; }
}
