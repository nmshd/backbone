using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.DomainEvents;
using Backbone.Tooling;

namespace Backbone.Modules.Tokens.Domain.Entities;

public class Token : Entity
{
    // ReSharper disable once UnusedMember.Local
    private Token()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
        Content = null!;
    }

    public Token(IdentityAddress createdBy, DeviceId createdByDevice, byte[] content, DateTime expiresAt, IdentityAddress? forIdentity = null)
    {
        Id = TokenId.New();

        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;

        CreatedAt = SystemTime.UtcNow;
        ExpiresAt = expiresAt;

        Content = content;
        ForIdentity = forIdentity;

        RaiseDomainEvent(new TokenCreatedDomainEvent(this));
    }

    public TokenId Id { get; set; }

    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }

    public IdentityAddress? ForIdentity { get; set; }

    public byte[] Content { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public static Expression<Func<Token, bool>> IsExpired =>
        challenge => challenge.ExpiresAt <= SystemTime.UtcNow;

    public static Expression<Func<Token, bool>> IsNotExpired =>
        challenge => challenge.ExpiresAt > SystemTime.UtcNow;
    
    public static Expression<Func<Token, bool>> CanBeCollectedBy(IdentityAddress? address)
    {
        return token => token.ForIdentity == null || address != null && (token.ForIdentity == address || token.CreatedBy == address);
    }

    public static Expression<Func<Token, bool>> WasCreatedBy(IdentityAddress identityAddress)
    {
        return t => t.CreatedBy == identityAddress.ToString();
    }
}
