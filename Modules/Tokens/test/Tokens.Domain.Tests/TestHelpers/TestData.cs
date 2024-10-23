using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Domain.Tests.TestHelpers;
public class TestData
{
    public static Token CreateToken(IdentityAddress createdBy, IdentityAddress? forIdentity, byte[]? password = null)
    {
        return new Token(createdBy, DeviceId.Parse("DVC1"), [], DateTime.Now.AddDays(1), forIdentity, password);
    }
}
