using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling.Extensions;
using NeoSmart.Utils;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.Tests.Helpers;

public class FileCreationHelper
{
    public static File CreateFile(IdentityAddress identityAddress)
    {
        var deviceId = CreateRandomDeviceId();
        var cipherHash = UrlBase64.Decode("AAAA");
        var ownerSignature = cipherHash;
        var encryptedProperties = cipherHash;
        var content = "Hello World!".GetBytes();

        return new File(identityAddress, deviceId, identityAddress, ownerSignature, cipherHash, content, content.LongLength, DateTime.Today.AddDays(1), encryptedProperties);
    }
}
