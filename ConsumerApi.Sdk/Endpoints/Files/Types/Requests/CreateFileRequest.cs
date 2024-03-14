using Backbone.Tooling.Extensions;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;

public class CreateFileRequest
{
    public required Stream Content { get; set; }
    public required string Owner { get; set; }
    public required string OwnerSignature { get; set; }

    public required string CipherHash { get; set; }

    public required DateTime ExpiresAt { get; set; }

    public required string EncryptedProperties { get; set; }

    public MultipartFormDataContent ToMultipartContent()
    {
        return new MultipartFormDataContent
        {
            { new StreamContent(Content), "content" },
            { new StringContent(Owner), "owner" },
            { new StringContent(OwnerSignature), "ownerSignature" },
            { new StringContent(CipherHash), "cipherHash" },
            { new StringContent(ExpiresAt.ToUniversalString()), "expiresAt" },
            { new StringContent(EncryptedProperties), "encryptedProperties" }
        };
    }
}
