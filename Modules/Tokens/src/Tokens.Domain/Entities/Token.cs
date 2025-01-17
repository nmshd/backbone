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

    // ReSharper disable once UnusedMember.Local
    private Token()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
        Content = null!;
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

    public bool CanBeCollectedUsingPassword(IdentityAddress? address, byte[]? password)
    {
        return
            Password == null ||
            password != null && Password.SequenceEqual(password) ||
            CreatedBy == address; // The owner shouldn't need a password to get the template
    }

    public void AnonymizeForIdentity(string didDomainName)
    {
        EnsureIsPersonalized();

        var anonymousIdentity = IdentityAddress.GetAnonymized(didDomainName);

        ForIdentity = anonymousIdentity;
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

    #endregion
}
