﻿using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling.Extensions;
using NeoSmart.Utils;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.Tests.Tests;

public class FileDeleteTests : AbstractTestsBase
{
    [Fact]
    public void FileCanBeDeletedByItsOwner()
    {
        var identity = CreateRandomIdentityAddress();
        var file = CreateFile(identity);

        var func = () => file.EnsureCanBeDeletedBy(identity);

        func.Should().NotThrow();
    }

    [Fact]
    public void FileCanNotBeDeletedByOthers()
    {
        var creatorIdentity = CreateRandomIdentityAddress();
        var otherIdentity = CreateRandomIdentityAddress();
        var file = CreateFile(creatorIdentity);

        var func = () => file.EnsureCanBeDeletedBy(otherIdentity);

        func.Should().Throw<DomainActionForbiddenException>();
    }

    private static File CreateFile(IdentityAddress identityAddress)
    {
        var deviceId = CreateRandomDeviceId();
        var cipherHash = UrlBase64.Decode("AAAA");
        var ownerSignature = cipherHash;
        var encryptedProperties = cipherHash;
        var content = "Hello World!".GetBytes();

        return new File(identityAddress, deviceId, identityAddress, ownerSignature, cipherHash, content, content.LongLength, DateTime.Today.AddDays(1), encryptedProperties);
    }
}
