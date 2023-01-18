using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.API.DTOs;

public class CreateFileDTO
{
    public IFormFile Content { get; set; }
    public IdentityAddress Owner { get; set; }
    public string OwnerSignature { get; set; }

    public string CipherHash { get; set; }

    public DateTime ExpiresAt { get; set; }

    public string EncryptedProperties { get; set; }
}