using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling.Extensions;
using NeoSmart.Utils;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.Tests.Helpers;

public static class TestDataGenerator
{
    public static File CreateFile()
    {
        var owner = CreateRandomIdentityAddress();
        return CreateFile(owner);
    }

    public static File CreateFile(IdentityAddress owner)
    {
        var deviceId = CreateRandomDeviceId();
        var cipherHash = UrlBase64.Decode("AAAA");
        var ownerSignature = cipherHash;
        var encryptedProperties = cipherHash;
        var content = "Hello World!".GetBytes();

        var file = new File(owner, deviceId, ownerSignature, cipherHash, content, content.LongLength, DateTime.Today.AddDays(1), encryptedProperties);
        file.ClearDomainEvents();

        return file;
    }
}
