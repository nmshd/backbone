using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations.Deprecated.BouncyCastle.Symmetric;
using Xunit;

namespace Backbone.Crypto.Tests.Tests.Implementations.Deprecated.BouncyCastle;

public class AesEncryptionHelperTests
{
    private readonly ISymmetricEncrypter _symmetricEncrypterUnderTest;

    #region Setup/Teardown

    public AesEncryptionHelperTests()
    {
        _symmetricEncrypterUnderTest = AesSymmetricEncrypter.CreateWith96BitIv128BitMac();
    }

    #endregion

    #region Decrypt

    [Fact]
    public void Decrypt_GivesCorrectResult_IfEncryptedTextWasGeneratedByEncrypt()
    {
        // Arrange
        var expected = "Test";
        var key = ConvertibleString.FromBase64("ZcR3a8iOzdQnC0nWHWIH6zJIwBAu2rz4EWNVtr98C/c=");
        var cipherText = _symmetricEncrypterUnderTest.Encrypt(ConvertibleString.FromUtf8(expected), key);

        // Act
        var actual = _symmetricEncrypterUnderTest.Decrypt(cipherText, key);

        // Assert
        Assert.Equal(expected, actual.Utf8Representation);
    }

    [Fact]
    public void Decrypt_GivesCorrectResult_IfEncryptedTextWasGeneratedByExternalProgram()
    {
        // Arrange
        var key = ConvertibleString.FromBase64("No14dfsch/cYYn1hAUc5vDAi/SEjOEhRjdEE0HVDxQA=");
        var cipherText = ConvertibleString.FromBase64("qm5n67I2KW8MGiaMFrISLrYOfscG+4SWQbqsGaiHqXk=");

        // Act
        var actual = _symmetricEncrypterUnderTest.Decrypt(cipherText, key);

        // Assert
        Assert.Equal("Test", actual.Utf8Representation);
    }

    [Fact]
    public void Decrypt_RaisesException_IfCipherTextIsEmpty()
    {
        // Arrange
        var key = ConvertibleString.FromBase64("No14dfsch/cYYn1hAUc5vDAi/SEjOEhRjdEE0HVDxQA=");
        var cipherText = ConvertibleString.FromBase64("");

        // Act&Assert
        var exception =
            Assert.Throws<ArgumentException>(() => _symmetricEncrypterUnderTest.Decrypt(cipherText, key));

        Assert.Equal("encryptedMessage", exception.ParamName);
    }

    [Theory]
    [InlineData("No14dfsch/cYYn1hAUc5vDAi/SEjOEhRjdEE0HVD")]
    [InlineData("dGVyc3d0d2V0cndlcndlcndyd3J3d2Vyd2Vyd2Vyd2Vyd2Vycnd3dw==")]
    public void Decrypt_RaisesException_IfKeyHasInvalidLength(string base64Key)
    {
        // Arrange
        var key = ConvertibleString.FromBase64(base64Key);
        var cipherText = ConvertibleString.FromBase64("qm5n67I2KW8MGiaMFrISLrYOfscG+4SWQbqsGaiHqXk=");

        // Act&Assert
        var exception =
            Assert.Throws<ArgumentException>(() => _symmetricEncrypterUnderTest.Decrypt(cipherText, key));

        Assert.Equal("key", exception.ParamName);
    }

    #endregion


    #region Encrypt

    [Fact]
    public void Encrypt_ShouldReturnDifferentResults_EvenThoughParametersAreTheSame()
    {
        // Arrange
        var plaintext = "Test";
        var key = ConvertibleString.FromBase64("ZcR3a8iOzdQnC0nWHWIH6zJIwBAu2rz4EWNVtr98C/c=");

        // Act
        var cipherText1 = _symmetricEncrypterUnderTest.Encrypt(ConvertibleString.FromUtf8(plaintext), key);
        var cipherText2 = _symmetricEncrypterUnderTest.Encrypt(ConvertibleString.FromUtf8(plaintext), key);

        // Assert
        Assert.False(cipherText1 == cipherText2);
    }

    [Theory]
    [InlineData("No14dfsch/cYYn1hAUc5vDAi/SEjOEhRjdEE0HVD")]
    [InlineData("dGVyc3d0d2V0cndlcndlcndyd3J3d2Vyd2Vyd2Vyd2Vyd2Vycnd3dw==")]
    public void Encrypt_RaisesException_IfKeyHasInvalidLength(string base64Key)
    {
        // Arrange
        var key = ConvertibleString.FromBase64(base64Key);

        // Act&Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _symmetricEncrypterUnderTest.Encrypt(ConvertibleString.FromUtf8("Test"), key));

        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void Decrypt_RaisesException_IfPlaintextIsEmpty()
    {
        // Arrange
        var key = ConvertibleString.FromBase64("No14dfsch/cYYn1hAUc5vDAi/SEjOEhRjdEE0HVDxQA=");

        // Act&Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _symmetricEncrypterUnderTest.Encrypt(ConvertibleString.FromUtf8(""), key));

        Assert.Equal("plaintext", exception.ParamName);
    }

    #endregion
}
