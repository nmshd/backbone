using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;

namespace Backbone.Modules.Tokens.Domain.Entities;

public class Token : Entity
{
    public const int MAX_PASSWORD_LENGTH = 200;
    public const int MAX_FAILED_ACCESS_ATTEMPTS_BEFORE_LOCK = 100;

    private readonly List<TokenAllocation> _allocations;
    private int _accessFailedCount;

    // ReSharper disable once UnusedMember.Local
    protected Token()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
        Details = null!;
        _allocations = null!;
        Version = null!;
    }

    public Token(IdentityAddress? createdBy, DeviceId? createdByDevice, byte[]? content, DateTime expiresAt, IdentityAddress? forIdentity = null, byte[]? password = null)
    {
        Id = TokenId.New();

        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;

        CreatedAt = SystemTime.UtcNow;
        ExpiresAt = expiresAt;

        Details = new TokenDetails { Id = Id, Content = content };
        ForIdentity = forIdentity;
        Password = password;

        _allocations = [];

        Version = null!; // This property is handled and initialized by the database

        RaiseDomainEvent(new TokenCreatedDomainEvent(this));
    }

    public TokenId Id { get; set; }

    public IdentityAddress? CreatedBy { get; set; }
    public DeviceId? CreatedByDevice { get; set; }

    public IdentityAddress? ForIdentity { get; private set; }
    public byte[]? Password { get; set; }

    public virtual TokenDetails Details { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public object Version { get; set; }

    public int AccessFailedCount
    {
        get => _accessFailedCount;
        private set
        {
            var wasLockedBeforeChange = IsLocked;

            _accessFailedCount = value;

            // since the access failed count can become higher than the limit from which on the token is considered
            // locked, we have to perform this check to avoid a TokenLockedDomainEvent being raised multiple times
            if (IsLocked && !wasLockedBeforeChange)
                RaiseDomainEvent(new TokenLockedDomainEvent(this));
        }
    }

    public virtual IReadOnlyList<TokenAllocation> Allocations => _allocations;
    public bool IsLocked => AccessFailedCount >= MAX_FAILED_ACCESS_ATTEMPTS_BEFORE_LOCK;
    public bool IsExpired => ExpiresAt < SystemTime.UtcNow;

    public TokenAccessResult TryToAccess(IdentityAddress? activeIdentity, DeviceId? device, byte[]? password)
    {
        if (HasOwner(activeIdentity))
            return TokenAccessResult.Ok;

        if (HasAllocationForIdentity(activeIdentity))
            return TokenAccessResult.Ok;

        if (IsExpired)
            return TokenAccessResult.Expired;

        if (IsLocked)
            return TokenAccessResult.Locked;

        if (!CanBeAccessAccordingToForIdentity(activeIdentity))
            return TokenAccessResult.ForIdentityDoesNotMatch;

        if (!IsPasswordCorrect(password))
        {
            AccessFailedCount++;

            return IsLocked ? TokenAccessResult.Locked : TokenAccessResult.WrongPassword;
        }

        if (activeIdentity == null)
            return TokenAccessResult.Ok;

        AllocateFor(activeIdentity, device);

        return TokenAccessResult.AllocationAdded;
    }

    private bool CanBeAccessAccordingToForIdentity(IdentityAddress? address)
    {
        return CreatedBy == address || ForIdentity == null || ForIdentity == address;
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

    private void EnsureIsPersonalized()
    {
        if (ForIdentity == null)
            throw new DomainException(DomainErrors.TokenNotPersonalized());
    }

    public void AnonymizeTokenAllocation(IdentityAddress address, string didDomainName)
    {
        var tokenAllocation = _allocations.Find(a => a.AllocatedBy == address) ?? throw new DomainException(DomainErrors.NoAllocationForIdentity());

        var anonymousIdentity = IdentityAddress.GetAnonymized(didDomainName);

        tokenAllocation.AllocatedBy = anonymousIdentity;
    }

    public void EnsureCanBeDeletedBy(IdentityAddress identityAddress)
    {
        if (CreatedBy != identityAddress)
            throw new DomainActionForbiddenException();
    }

    #region Expressions

    public static Expression<Func<Token, bool>> IsNotExpired =>
        challenge => challenge.ExpiresAt > SystemTime.UtcNow;

    public static Expression<Func<Token, bool>> WasCreatedBy(IdentityAddress identityAddress)
    {
        return t => t.CreatedBy == identityAddress.ToString();
    }

    public static Expression<Func<Token, bool>> HasId(TokenId id)
    {
        return r => r.Id == id;
    }

    public static Expression<Func<Token, bool>> IsFor(IdentityAddress identityAddress)
    {
        return token => token.ForIdentity == identityAddress;
    }

    public static Expression<Func<Token, bool>> HasAllocationFor(IdentityAddress identityAddress)
    {
        return token => token.Allocations.Any(allocation => allocation.AllocatedBy == identityAddress);
    }

    #endregion

    public void ResetAccessFailedCount()
    {
        AccessFailedCount = 0;
    }
}

public enum TokenAccessResult
{
    Ok,
    AllocationAdded,
    WrongPassword,
    ForIdentityDoesNotMatch,
    Locked,
    Expired
}

public class TokenDetails
{
    public required TokenId Id { get; init; } = null!;
    public required byte[]? Content { get; init; }
}
