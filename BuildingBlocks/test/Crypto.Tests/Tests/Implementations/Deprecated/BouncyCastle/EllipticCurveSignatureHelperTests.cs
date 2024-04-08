using Backbone.Crypto.Implementations.Deprecated.BouncyCastle.Asymmetric;
using Xunit;

namespace Backbone.Crypto.Tests.Tests.Implementations.Deprecated.BouncyCastle;

public class EllipticCurveSignatureHelperTests
{
    private readonly EllipticCurveSignatureHelper _signatureHelper;

    #region Test Data

    private readonly ConvertibleString _validPublicKey = ConvertibleString.FromBase64(
        "MIGbMBAGByqGSM49AgEGBSuBBAAjA4GGAAQAHOZSL2+EVFUOxiT1lTaS+FQCc9D2Qm8x5Exgg73RaOBzq/7wNgEK1zimVbajsUFI/331EIb7BPAePUodGcz7UcYB8eLkEj6j5QXIKghwOP1x7PSz27yjWGJJD6AWcvbKZrVaxpLSYvjNYpwJUrU58a//1Qt2QBW1ku34VvRIhhwW3lA=");

    private readonly ConvertibleString _validPrivateKey = ConvertibleString.FromBase64(
        "MIHuAgEAMBAGByqGSM49AgEGBSuBBAAjBIHWMIHTAgEBBEIBAkkvXspQwTUbnqQE0PO5Xtnb9F223zF7XP0Y1NXxbaVQassO16X118JTCGOosEe3j28oVXQRbWGyEkQf6f0kv1ShgYkDgYYABAAc5lIvb4RUVQ7GJPWVNpL4VAJz0PZCbzHkTGCDvdFo4HOr/vA2AQrXOKZVtqOxQUj/ffUQhvsE8B49Sh0ZzPtRxgHx4uQSPqPlBcgqCHA4/XHs9LPbvKNYYkkPoBZy9spmtVrGktJi+M1inAlStTnxr//VC3ZAFbWS7fhW9EiGHBbeUA==");

    private readonly ConvertibleString _validSignature = ConvertibleString.FromBase64(
        "MIGIAkIAgzWTIjNaFJF7eHylzvDM1Zp2wjsjQ6rB/FDSaBVTLALvGCu+DV8FeXI4PrZ69VgQq8jp4IKpIyqShYjSf5QbRkYCQgHoulTSyLd4po/sgcFJYrKnotPEJtF6eUZA3/Su5cjc5UrTq2d6RbRnpmcOkxeQEmyRbKZ+WIub6AgJTFQ0d9Isag==");

    #endregion

    #region Setup and Teardown

    public EllipticCurveSignatureHelperTests()
    {
        _signatureHelper = EllipticCurveSignatureHelper.CreateSha512WithEcdsa();
    }

    #endregion

    #region Tests

    #region VerifySignature

    [Fact]
    public void VerifySignature_ShouldReturnTrue_IfSignatureIsValid()
    {
        // Arrange
        var data = ConvertibleString.FromUtf8("Test");

        // Act
        var isValid = _signatureHelper.VerifySignature(data, _validSignature, _validPublicKey);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifySignature_ShouldReturnFalse_IfSignatureIsInvalid()
    {
        // Arrange
        var data = ConvertibleString.FromUtf8("Test");
        var invalidSignature = ConvertibleString.FromBase64(
            "MIGIAkIAgzWTIjNaFJF7eHylzwDM1Zp2wjsjQ6rB/FDSaBVTLALvGCu+DV8FeXI4PrZ69VgQq8jp4IKpIyqShYjSf5QbRkYCQgHoulTSyLd4po/sgcFJYrKnotPEJtF6eUZA3/Su5cjc5UrTq2d6RbRnpmcOkxeQEmyRbKZ+WIub6AgJTFQ0d9Isag==");

        // Act
        var isValid = _signatureHelper.VerifySignature(data, invalidSignature, _validPublicKey);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifySignature_ShouldRaiseException_IfPublicKeyIsInvalid()
    {
        // Arrange
        var data = ConvertibleString.FromUtf8("Test");
        var invalidPublicKey = ConvertibleString.FromBase64("AA==");

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            _signatureHelper.VerifySignature(data, _validSignature, invalidPublicKey));

        // Assert
        Assert.Equal("publicKey", exception.ParamName);
    }

    [Fact]
    public void VerifySignature_ShouldRaiseException_IfMessageIsDifferent()
    {
        // Arrange
        var differentMessage = ConvertibleString.FromUtf8("Test2");

        // Act
        var isValid = _signatureHelper.VerifySignature(differentMessage, _validSignature, _validPublicKey);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifySignature_ShouldReturnCorrectResult_IfSignatureWasGeneratedByExternalTool()
    {
        // Arrange
        var data = ConvertibleString.FromUtf8("Test");
        var signature = ConvertibleString.FromBase64(
            "MIGIAkIAgzWTIjNaFJF7eHylzvDM1Zp2wjsjQ6rB/FDSaBVTLALvGCu+DV8FeXI4PrZ69VgQq8jp4IKpIyqShYjSf5QbRkYCQgHoulTSyLd4po/sgcFJYrKnotPEJtF6eUZA3/Su5cjc5UrTq2d6RbRnpmcOkxeQEmyRbKZ+WIub6AgJTFQ0d9Isag==");

        // Act
        var isValid = _signatureHelper.VerifySignature(data, signature, _validPublicKey);

        // Assert
        Assert.True(isValid);
    }

    #endregion

    #region GetSignature

    [Fact]
    public void GetSignature_CreatesCorrectResult()
    {
        // Arrange
        var plaintext = ConvertibleString.FromUtf8("Test");

        // Act
        var signature = _signatureHelper.CreateSignature(_validPrivateKey, plaintext);
        var isValid = _signatureHelper.VerifySignature(plaintext, signature, _validPublicKey);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void GetSignature_ThrowsException_IfPrivateKeyIsInvalid()
    {
        // Arrange
        var plaintext = ConvertibleString.FromUtf8("Test");
        var invalidPrivateKey = ConvertibleString.FromBase64(
            "MIHuAgEAMBAGByqGSM49AgEGBSuBBAAjBIHWMIHTAgEBBEIBAkkvXspQwTUbnqQE0PO5Xtnb9F223zF7XP0Y1NXxbaVQassO16X118JTCGOosEe3j28oVXQRbWGyEkQf6f0kv1ShgYkDgYYABAAc5lIvb4RUVQ7GJPWVNpL4VAJz0PZCbzHkTGCDvdFo4HOr/vA2AQrXOKZVtqOxQUj/ffUQhvsE8B49Sh0ZzPtRxgHx4uQSPqPlBcgqCHA4/XHs9LPbvKNYYkkPoBZy9spmtVrGktJi+M1inAlStTnxr//VC3ZAFbWS7fhW9EiGHBbe");

        // Act
        var exception =
            Assert.Throws<ArgumentException>(() => _signatureHelper.CreateSignature(invalidPrivateKey, plaintext));
        Assert.Equal("privateKey", exception.ParamName);
        // Assert
    }

    #endregion

    #endregion
}
