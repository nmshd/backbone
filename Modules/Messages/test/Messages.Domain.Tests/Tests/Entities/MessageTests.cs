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
    public void Message_should_return_a_decrypted_body_provided_correct_symmetric_key_has_been_passed_as_a_parameter()
    {
        // Arrange
        var address = TestDataGenerator.CreateRandomIdentityAddress();
        var deviceId = TestDataGenerator.CreateRandomDeviceId();
        var body = new byte[] { 1, 2, 3, 4 };
        var attachments = new List<Attachment>();
        var recipents = new List<RecipientInformation>();

        var message = new Message(address, deviceId, null, body, attachments, recipents);

        // Act
        var symmetricKey = "code";
        var decryptedBody = message.DecryptBody(symmetricKey);

        // Assert
        decryptedBody.Should().Be("01-02-03-04");
    }


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


    //[Fact]
    //public void Should_decrypt_body_with_given_symmetric_key()
    //{
    //    // Arrange
    //    var aesEncryptionHelper = AesSymmetricEncrypter.CreateWith96BitIv128BitMac();

    //    var address = TestDataGenerator.CreateRandomIdentityAddress();
    //    var deviceId = TestDataGenerator.CreateRandomDeviceId();
    //    var body = new byte[] { 0, 0, 7};
    //    var attachments = new List<Attachment>();
    //    var recipents = new List<RecipientInformation>();

    //    var message = new Message(address, deviceId, null, body, attachments, recipents);

    //    var testString = BitConverter.ToString(body);

    //    // Act
    //    var keyGeneratorUnderTest = new KeyGenerator();
    //    var symmetricKey = keyGeneratorUnderTest.GenerateSymmetricKey(256);
    //    var encryptedMessage = aesEncryptionHelper.Encrypt(ConvertibleString.FromUtf8(testString), symmetricKey);
    //    var result = message.DecryptBodyWithSymmetricKey(encryptedMessage, symmetricKey);

    //    // Assert
    //    result.Utf8Representation.Should().Be(testString);
    //}
}

