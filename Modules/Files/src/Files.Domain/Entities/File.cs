using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;

namespace Backbone.Modules.Files.Domain.Entities;

public class File : Entity
{
    // ReSharper disable once UnusedMember.Local
    private File()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
        ModifiedBy = null!;
        ModifiedByDevice = null!;
        Owner = null!;
        OwnerSignature = null!;
        CipherHash = null!;
        Content = null!;
        EncryptedProperties = null!;
        OwnershipToken = null!;
    }

    public File(IdentityAddress createdBy, DeviceId createdByDevice, byte[] ownerSignature, byte[] cipherHash, byte[] content, long cipherSize, DateTime expiresAt,
        byte[] encryptedProperties)
    {
        Id = FileId.New();

        CreatedAt = ModifiedAt = SystemTime.UtcNow;
        Owner = CreatedBy = ModifiedBy = createdBy;
        CreatedByDevice = ModifiedByDevice = createdByDevice;

        OwnerSignature = ownerSignature;

        OwnershipToken = FileOwnershipToken.New();

        CipherHash = cipherHash;
        CipherSize = cipherSize;

        Content = content;

        ExpiresAt = expiresAt;

        EncryptedProperties = encryptedProperties;

        RaiseDomainEvent(new FileUploadedDomainEvent(this));
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

    public IdentityAddress Owner { get; set; }
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

    public FileOwnershipToken OwnershipToken { get; set; }
    public bool BlockOwnershipClaims { get; set; } = false;

    public void EnsureCanBeDeletedBy(IdentityAddress identityAddress)
    {
        if (CreatedBy != identityAddress) throw new DomainActionForbiddenException();
    }

    public static Expression<Func<File, bool>> IsExpired =>
        file => file.ExpiresAt <= SystemTime.UtcNow;

    public static Expression<Func<File, bool>> IsNotExpired =>
        file => file.ExpiresAt > SystemTime.UtcNow;

    public static Expression<Func<File, bool>> IsDeleted =>
        file => file.DeletedAt != null;

    public static Expression<Func<File, bool>> IsNotDeleted =>
        file => file.DeletedAt == null;

    public static Expression<Func<File, bool>> WasCreatedBy(IdentityAddress identityAddress)
    {
        return i => i.CreatedBy == identityAddress.ToString();
    }

    public string RegenerateOwnershipToken()
    {
        OwnershipToken = FileOwnershipToken.New();
        BlockOwnershipClaims = false;
        return OwnershipToken.Value;
    }

    public string ClaimOwnership(string ownershipToken, IdentityAddress newOwnerAddress)
    {
        if (BlockOwnershipClaims) throw new DomainActionForbiddenException();

        if (OwnershipToken.Value != ownershipToken)
        {
            BlockOwnershipClaims = true;
            RaiseDomainEvent(new FileOwnershipIsLockedEvent(Id.Value, Owner.Value));
            throw new DomainException(new DomainError("error.module.files.invalidFileOwnershipToken", "The file ownership token is invalid."));
        }

        Owner = IdentityAddress.Parse(newOwnerAddress);
        return RegenerateOwnershipToken();
    }
}
