namespace Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Responses;

public class CreateFileResponse
{
    public required string Id { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }

    public required DateTime ModifiedAt { get; set; }
    public required string ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public required string Owner { get; set; }
    public required byte[] OwnerSignature { get; set; }

    public required string OwnershipToken { get; set; }
    public required bool OwnershipIsLocked { get; set; }

    public required long CipherSize { get; set; }
    public required byte[] CipherHash { get; set; }

    public required DateTime ExpiresAt { get; set; }
}
