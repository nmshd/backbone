using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Tokens.Domain.Entities;

public class Token
{
#pragma warning disable CS8618
    private Token() { }
#pragma warning restore CS8618

    public Token(IdentityAddress createdBy, DeviceId createdByDevice, byte[] content, DateTime expiresAt)
    {
        Id = TokenId.New();

        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;

        CreatedAt = SystemTime.UtcNow;
        ExpiresAt = expiresAt;

        Content = content;
    }

    public TokenId Id { get; set; }

    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }

    public byte[] Content { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public void LoadContent(byte[] content)
    {
        if (Content != null)
            throw new Exception("Cannot change the content of a token.");

        Content = content;
    }

    public static Expression<Func<Token, bool>> IsExpired =>
        challenge => challenge.ExpiresAt <= SystemTime.UtcNow;

    public static Expression<Func<Token, bool>> IsNotExpired =>
        challenge => challenge.ExpiresAt > SystemTime.UtcNow;

    public static Expression<Func<Token, bool>> WasCreatedBy(IdentityAddress identityAddress)
    {
        return t => t.CreatedBy == identityAddress;
    }
}
