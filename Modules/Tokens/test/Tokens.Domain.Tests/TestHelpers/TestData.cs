using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Domain.Tests.TestHelpers;

public class TestData
{
    public static Token CreateToken(IdentityAddress? createdBy = null, IdentityAddress? forIdentity = null, byte[]? password = null)
    {
        createdBy ??= CreateRandomIdentityAddress();
        var createdByDevice = CreateRandomDeviceId();
        return new Token(createdBy, createdByDevice, [], DateTime.Now.AddDays(1), forIdentity, password);
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
