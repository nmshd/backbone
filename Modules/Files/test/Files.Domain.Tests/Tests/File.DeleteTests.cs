using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling.Extensions;
using NeoSmart.Utils;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.Tests.Tests;

public class FileDeleteTests : AbstractTestsBase
{
    [Fact]
    public void File_can_be_deleted_by_its_owner()
    {
        var identity = CreateRandomIdentityAddress();
        var file = CreateFile(identity);

        var acting = () => file.EnsureCanBeDeletedBy(identity);

        acting.ShouldNotThrow();
    }

    [Fact]
    public void File_can_not_be_deleted_by_others()
    {
        var creatorIdentity = CreateRandomIdentityAddress();
        var otherIdentity = CreateRandomIdentityAddress();
        var file = CreateFile(creatorIdentity);

        var acting = () => file.EnsureCanBeDeletedBy(otherIdentity);

        acting.ShouldThrow<DomainActionForbiddenException>();
    }

    private static File CreateFile(IdentityAddress identityAddress)
    {
        var deviceId = CreateRandomDeviceId();
        var cipherHash = UrlBase64.Decode("AAAA");
        var ownerSignature = cipherHash;
        var encryptedProperties = cipherHash;
        var content = "Hello World!".GetBytes();

        return new File(identityAddress, deviceId, ownerSignature, cipherHash, content, content.LongLength, DateTime.Today.AddDays(1), encryptedProperties);
    }
}
