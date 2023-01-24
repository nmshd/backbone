using System.Linq.Expressions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Files.Domain.Entities;

public class FileMetadata
{
    public FileMetadata(IdentityAddress createdBy, DeviceId createdByDevice, IdentityAddress owner, byte[] ownerSignature, byte[] cipherHash, long cipherSize, DateTime expiresAt, byte[] encryptedProperties)
    {
        Id = FileId.New();

        CreatedAt = ModifiedAt = SystemTime.UtcNow;
        CreatedBy = ModifiedBy = createdBy;
        CreatedByDevice = ModifiedByDevice = createdByDevice;

        Owner = owner;
        OwnerSignature = ownerSignature;

        CipherHash = cipherHash;
        CipherSize = cipherSize;

        ExpiresAt = expiresAt;

        EncryptedProperties = encryptedProperties;
    }

    public FileId Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }

    public DateTime ModifiedAt { get; set; }
    public IdentityAddress ModifiedBy { get; set; }
    public DeviceId ModifiedByDevice { get; set; }

    public DateTime? DeletedAt { get; set; }
    public IdentityAddress? DeletedBy { get; set; }
    public DeviceId? DeletedByDevice { get; set; }

    public IdentityAddress? Owner { get; set; }
    public byte[] OwnerSignature { get; set; }

    public long CipherSize { get; set; }
    public byte[] CipherHash { get; set; }

    public DateTime ExpiresAt { get; set; }

    public byte[] EncryptedProperties { get; set; }

    public static Expression<Func<FileMetadata, bool>> IsExpired =>
        file => file.ExpiresAt <= SystemTime.UtcNow;

    public static Expression<Func<FileMetadata, bool>> IsNotExpired =>
        file => file.ExpiresAt > SystemTime.UtcNow;

    public static Expression<Func<FileMetadata, bool>> IsDeleted =>
        file => file.DeletedAt != null;

    public static Expression<Func<FileMetadata, bool>> IsNotDeleted =>
        file => file.DeletedAt == null;
}
