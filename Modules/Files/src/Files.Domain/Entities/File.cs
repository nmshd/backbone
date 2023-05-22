using System.Linq.Expressions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Backbone.Modules.Files.Domain.Entities;

public class File
{
#pragma warning disable CS8618
    private File() { }
#pragma warning restore CS8618

    public File(IdentityAddress createdBy, DeviceId createdByDevice, IdentityAddress owner, byte[] ownerSignature, byte[] cipherHash, byte[] content, long cipherSize, DateTime expiresAt, byte[] encryptedProperties)
    {
        Id = FileId.New();

        CreatedAt = ModifiedAt = SystemTime.UtcNow;
        CreatedBy = ModifiedBy = createdBy;
        CreatedByDevice = ModifiedByDevice = createdByDevice;

        Owner = owner;
        OwnerSignature = ownerSignature;

        CipherHash = cipherHash;
        CipherSize = cipherSize;

        Content = content;

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

    public byte[] Content { get; private set; }

    public void LoadContent(byte[] content)
    {
        if (Content != null)
        {
            throw new InvalidOperationException($"The Content of the file {Id} is already filled. It is not possible to change it.");
        }
        Content = content;
    }

    public DateTime ExpiresAt { get; set; }

    public byte[] EncryptedProperties { get; set; }

    public static Expression<Func<File, bool>> IsExpired =>
        file => file.ExpiresAt <= SystemTime.UtcNow;

    public static Expression<Func<File, bool>> IsNotExpired =>
        file => file.ExpiresAt > SystemTime.UtcNow;

    public static Expression<Func<File, bool>> IsDeleted =>
        file => file.DeletedAt != null;

    public static Expression<Func<File, bool>> IsNotDeleted =>
        file => file.DeletedAt == null;
}
