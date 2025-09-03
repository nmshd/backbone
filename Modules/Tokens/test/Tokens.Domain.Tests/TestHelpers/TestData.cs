using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Tooling;

namespace Backbone.Modules.Tokens.Domain.Tests.TestHelpers;

public class TestData
{
    public static Token CreateTokenWithoutContent(IdentityAddress? forIdentity = null, byte[]? password = null, DateTime? expiresAt = null)
    {
        expiresAt ??= SystemTime.UtcNow.AddMinutes(1);
        return new Token(null, null, null, expiresAt.Value, forIdentity, password);
    }

    public static Token CreateToken(IdentityAddress? createdBy = null, IdentityAddress? forIdentity = null, byte[]? password = null)
    {
        createdBy ??= CreateRandomIdentityAddress();
        var createdByDevice = CreateRandomDeviceId();
        return new Token(createdBy, createdByDevice, CreateRandomBytes(), SystemTime.UtcNow.AddDays(1), forIdentity, password);
    }

    public static Token CreateLockedToken()
    {
        byte[] wrongPassword = [2];
        var unauthorizedIdentity = CreateRandomIdentityAddress();
        var deviceOfUnauthorizedIdentity = CreateRandomDeviceId();

        var token = CreateToken(password: [1]);

        for (var i = 0; i < Token.MAX_FAILED_ACCESS_ATTEMPTS_BEFORE_LOCK; i++)
        {
            token.TryToAccess(unauthorizedIdentity, deviceOfUnauthorizedIdentity, wrongPassword);
        }

        return token;
    }
}
