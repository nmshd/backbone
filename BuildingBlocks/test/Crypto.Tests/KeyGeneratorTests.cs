using Backbone.Crypto.Implementations.Deprecated.BouncyCastle;
using Backbone.Crypto.Implementations.Deprecated.BouncyCastle.Symmetric;
using Xunit;

namespace Backbone.Crypto.Tests;

public class KeyGeneratorTests
{
    private readonly KeyGenerator _keyGeneratorUnderTest;

    #region GenerateSymmetricKey

    [Fact]
    public void GenerateSymmetricKey_ShouldGenerateKeyThatCanBeUsedForSymmetricEncryption()
    {
        // Arrange
        var aesEncryptionHelper = AesSymmetricEncrypter.CreateWith96BitIv128BitMac();
        var testString = "Test";

        // Act
        var symmetricKey = _keyGeneratorUnderTest.GenerateSymmetricKey(256);
        var encrypted = aesEncryptionHelper.Encrypt(ConvertibleString.FromUtf8(testString), symmetricKey);
        var result = aesEncryptionHelper.Decrypt(encrypted, symmetricKey);

        // Assert
        Assert.Equal(testString, result.Utf8Representation);
    }

    #endregion

    #region Setup/Teardown

    public KeyGeneratorTests()
    {
        _keyGeneratorUnderTest = new KeyGenerator();
    }

    #endregion

    #region DeriveSymmetricKeyWithEcdh

    [Fact]
    public void DeriveSymmetricKeyWithEcdh_ShouldGenerateKeyWithCorrectLength()
    {
        // Arrange
        var publicKey = ConvertibleString.FromBase64(
            "MIGbMBAGByqGSM49AgEGBSuBBAAjA4GGAAQAHOZSL2+EVFUOxiT1lTaS+FQCc9D2Qm8x5Exgg73RaOBzq/7wNgEK1zimVbajsUFI/331EIb7BPAePUodGcz7UcYB8eLkEj6j5QXIKghwOP1x7PSz27yjWGJJD6AWcvbKZrVaxpLSYvjNYpwJUrU58a//1Qt2QBW1ku34VvRIhhwW3lA=");
        var privateKey = ConvertibleString.FromBase64(
            "MIHuAgEAMBAGByqGSM49AgEGBSuBBAAjBIHWMIHTAgEBBEIBAkkvXspQwTUbnqQE0PO5Xtnb9F223zF7XP0Y1NXxbaVQassO16X118JTCGOosEe3j28oVXQRbWGyEkQf6f0kv1ShgYkDgYYABAAc5lIvb4RUVQ7GJPWVNpL4VAJz0PZCbzHkTGCDvdFo4HOr/vA2AQrXOKZVtqOxQUj/ffUQhvsE8B49Sh0ZzPtRxgHx4uQSPqPlBcgqCHA4/XHs9LPbvKNYYkkPoBZy9spmtVrGktJi+M1inAlStTnxr//VC3ZAFbWS7fhW9EiGHBbeUA==");

        // Act
        var symmetricKey = _keyGeneratorUnderTest.DeriveSymmetricKeyWithEcdh(privateKey, publicKey, 256);

        // Assert
        Assert.Equal(256, symmetricKey.SizeInBits);
    }

    [Fact]
    public void DeriveSymmetricKeyWithEcdh_ShouldGenerateKeyThatCanBeUsedForSymmetricEncryption()
    {
        // Arrange
        var publicKey = ConvertibleString.FromBase64(
            "MIGbMBAGByqGSM49AgEGBSuBBAAjA4GGAAQAHOZSL2+EVFUOxiT1lTaS+FQCc9D2Qm8x5Exgg73RaOBzq/7wNgEK1zimVbajsUFI/331EIb7BPAePUodGcz7UcYB8eLkEj6j5QXIKghwOP1x7PSz27yjWGJJD6AWcvbKZrVaxpLSYvjNYpwJUrU58a//1Qt2QBW1ku34VvRIhhwW3lA=");
        var privateKey = ConvertibleString.FromBase64(
            "MIHuAgEAMBAGByqGSM49AgEGBSuBBAAjBIHWMIHTAgEBBEIBAkkvXspQwTUbnqQE0PO5Xtnb9F223zF7XP0Y1NXxbaVQassO16X118JTCGOosEe3j28oVXQRbWGyEkQf6f0kv1ShgYkDgYYABAAc5lIvb4RUVQ7GJPWVNpL4VAJz0PZCbzHkTGCDvdFo4HOr/vA2AQrXOKZVtqOxQUj/ffUQhvsE8B49Sh0ZzPtRxgHx4uQSPqPlBcgqCHA4/XHs9LPbvKNYYkkPoBZy9spmtVrGktJi+M1inAlStTnxr//VC3ZAFbWS7fhW9EiGHBbeUA==");
        var aesEncryptionHelper = AesSymmetricEncrypter.CreateWith96BitIv128BitMac();
        var testString = "Test";

        // Act
        var symmetricKey = _keyGeneratorUnderTest.DeriveSymmetricKeyWithEcdh(privateKey, publicKey, 256);
        var encrypted = aesEncryptionHelper.Encrypt(ConvertibleString.FromUtf8(testString), symmetricKey);
        var result = aesEncryptionHelper.Decrypt(encrypted, symmetricKey);

        // Assert
        Assert.Equal(testString, result.Utf8Representation);
    }

    #endregion
}
