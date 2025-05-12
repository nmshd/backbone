using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.DTOs;

public class FileMetadataDTO
{
    public FileMetadataDTO(File file)
    {
        Id = file.Id;
        CreatedAt = file.CreatedAt;
        CreatedBy = file.CreatedBy;
        CreatedByDevice = file.CreatedByDevice;
        ModifiedAt = file.ModifiedAt;
        ModifiedBy = file.ModifiedBy;
        ModifiedByDevice = file.ModifiedByDevice;
        DeletedAt = file.DeletedAt;
        DeletedBy = file.DeletedBy?.Value;
        DeletedByDevice = file.DeletedByDevice?.Value;
        Owner = file.Owner;
        OwnerSignature = file.OwnerSignature;
        BlockOwnershipClaims = file.BlockOwnershipClaims;
        CipherSize = file.CipherSize;
        CipherHash = file.CipherHash;
        ExpiresAt = file.ExpiresAt;
        EncryptedProperties = file.EncryptedProperties;
    }

    public string Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedByDevice { get; set; }

    public DateTime ModifiedAt { get; set; }
    public string ModifiedBy { get; set; }
    public string ModifiedByDevice { get; set; }

    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeletedByDevice { get; set; }

    public string Owner { get; set; }
    public byte[] OwnerSignature { get; set; }

    public bool BlockOwnershipClaims { get; set; }

    public long CipherSize { get; set; }
    public byte[] CipherHash { get; set; }

    public DateTime ExpiresAt { get; set; }

    public byte[] EncryptedProperties { get; set; }
}
