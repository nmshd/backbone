using Backbone.Crypto;
using Backbone.Crypto.Implementations.Deprecated.BouncyCastle;
using Backbone.Crypto.Implementations.Deprecated.BouncyCastle.Symmetric;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Tests.Entities;

public class MessageTests
{
    [Fact]
    public void GenerateSymmetricKey_ShouldGenerateKeyThatCanBeUsedForSymmetricEncryption()
    {
        // Arrange
        var aesEncryptionHelper = AesSymmetricEncrypter.CreateWith96BitIv128BitMac();
        var testString = "Test";

        // Act
        var keyGeneratorUnderTest = new KeyGenerator();
        var symmetricKey = keyGeneratorUnderTest.GenerateSymmetricKey(256);
        var encrypted = aesEncryptionHelper.Encrypt(ConvertibleString.FromUtf8(testString), symmetricKey);
        var result = aesEncryptionHelper.Decrypt(encrypted, symmetricKey);

        // Assert
        testString.Should().Be(result.Utf8Representation);
    }
}
