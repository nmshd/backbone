using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.DomainEvents;
using Backbone.Tooling;

namespace Backbone.Modules.Tokens.Domain.Entities;

public class Token : Entity
{
    public const int MAX_PASSWORD_LENGTH = 200;
    private const int MAX_FAILED_ACCESS_ATTEMPTS = 100;

    private readonly List<TokenAllocation> _allocations;
    private int _accessFailedCount;

    // ReSharper disable once UnusedMember.Local
    private Token()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
        Content = null!;
        _allocations = null!;
    }

    public Token(IdentityAddress createdBy, DeviceId createdByDevice, byte[] content, DateTime expiresAt, IdentityAddress? forIdentity = null, byte[]? password = null)
    {
        Id = TokenId.New();

        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;

        CreatedAt = SystemTime.UtcNow;
        ExpiresAt = expiresAt;

        Content = content;
        ForIdentity = forIdentity;
        Password = password;

        _allocations = [];

        RaiseDomainEvent(new TokenCreatedDomainEvent(this));
    }

    public TokenId Id { get; set; }

    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }

    public IdentityAddress? ForIdentity { get; private set; }
    public byte[]? Password { get; set; }

    public byte[] Content { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public int AccessFailedCount
    {
        get => _accessFailedCount;
        private set
        {
            if (IsLocked) return;

            _accessFailedCount = value;

            if (IsLocked)
            {
                RaiseDomainEvent(new TokenLockedDomainEvent(this));
            }
        }
    }

    public IReadOnlyList<TokenAllocation> Allocations => _allocations;
    public bool IsLocked => AccessFailedCount >= MAX_FAILED_ACCESS_ATTEMPTS;

    public TokenAccessResult TryToAccess(IdentityAddress? address, DeviceId? device, byte[]? password)
    {
        if (HasOwner(address))
            return TokenAccessResult.Ok;

        if (HasAllocationForIdentity(address))
            return TokenAccessResult.Ok;

        if (IsLocked)
            return TokenAccessResult.Locked;

        if (!IsPasswordCorrect(password))
        {
            AccessFailedCount++;

            return IsLocked ? TokenAccessResult.Locked : TokenAccessResult.WrongPassword;
        }

        if (address == null)
            return TokenAccessResult.Ok;

        AllocateFor(address, device);

        return TokenAccessResult.AllocationAdded;
    }

    private void AllocateFor(IdentityAddress address, DeviceId? device)
    {
        var allocation = new TokenAllocation(this, address, device!);
        _allocations.Add(allocation);
    }

    private bool HasAllocationForIdentity(IdentityAddress? address)
    {
        return address != null && Allocations.Any(a => a.AllocatedBy == address);
    }

    private bool HasOwner(IdentityAddress? address)
    {
        return CreatedBy == address;
    }

    private bool IsPasswordCorrect(byte[]? password)
    {
        return Password == null || password != null && Password.SequenceEqual(password);
    }

    public void AnonymizeForIdentity(string didDomainName)
    {
        EnsureIsPersonalized();

        var anonymousIdentity = IdentityAddress.GetAnonymized(didDomainName);

        ForIdentity = anonymousIdentity;
    }

    public void AnonymizeTokenAllocation(IdentityAddress address, string didDomainName)
    {
        var tokenAllocation = _allocations.Find(a => a.AllocatedBy == address) ?? throw new DomainException(DomainErrors.NoAllocationForIdentity());

        var anonymousIdentity = IdentityAddress.GetAnonymized(didDomainName);

        tokenAllocation.AllocatedBy = anonymousIdentity;
    }

    public void EnsureCanBeDeletedBy(IdentityAddress identityAddress)
    {
        if (CreatedBy != identityAddress) throw new DomainActionForbiddenException();
    }

    public void EnsureIsPersonalized()
    {
        if (ForIdentity == null) throw new DomainException(DomainErrors.TokenNotPersonalized());
    }

    #region Expressions

    public static Expression<Func<Token, bool>> IsNotExpired =>
        challenge => challenge.ExpiresAt > SystemTime.UtcNow;

    public static Expression<Func<Token, bool>> CanBeCollectedBy(IdentityAddress? address)
    {
        return token => token.ForIdentity == null || token.ForIdentity == address || token.CreatedBy == address;
    }

    public static Expression<Func<Token, bool>> WasCreatedBy(IdentityAddress identityAddress)
    {
        return t => t.CreatedBy == identityAddress.ToString();
    }

    public static Expression<Func<Token, bool>> HasId(TokenId id)
    {
        return r => r.Id == id;
    }

    public static Expression<Func<Token, bool>> CanBeCollectedWithPassword(IdentityAddress address, byte[]? password)
    {
        return token =>
            token.Password == null ||
            token.Password == password ||
            token.CreatedBy == address; // The owner shouldn't need a password to get the template
    }

    public static Expression<Func<Token, bool>> IsFor(IdentityAddress identityAddress)
    {
        return token => token.ForIdentity == identityAddress;
    }

    private static Expression<Func<Token, bool>> WasCreatedBy(string createdBy)
    {
        return token => token.CreatedBy == createdBy;
    }

    public static Expression<Func<Token, bool>> HasAllocationFor(IdentityAddress identityAddress)
    {
        return token => token.CreatedBy == identityAddress || token.Allocations.Any(allocation => allocation.AllocatedBy == identityAddress);
    }

    #endregion
}

public enum TokenAccessResult
{
    Ok,
    AllocationAdded,
    WrongPassword,
    Locked
}
