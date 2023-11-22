using Backbone.Crypto.Implementations;
using NSec.Cryptography;
using Xunit;

namespace Backbone.Crypto.Tests.Tests.Implementations;

public class SignatureHelperTests : IDisposable
{
    private SignatureHelper _signatureHelper;

    #region Test Data

    private readonly ConvertibleString _validPublicKey =
        ConvertibleString.FromBase64("pvJlACBFHHNOnGjpEE0ZTGSm1UDkKJmxV8PPQb87JBM=");

    private readonly ConvertibleString _validPrivateKey =
        ConvertibleString.FromBase64("0G8hUwUHf/BLg+BX/MXihPQCi5G1cYPmPxo9/OfWAXE=");

    private readonly ConvertibleString _validSignature =
        ConvertibleString.FromBase64(
            "Yzi2JHvS46CrJrJwuEq4TOY5LCQL8AAQjw6JojULaZuDCFKWBORS1cEWYpUnGXEjK/VvXTsoJ9J1S7JTMfzMAQ==");

    #endregion

    #region Setup and Teardown

    public SignatureHelperTests()
    {
        _signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();
    }

    public void Dispose()
    {
        _signatureHelper = null;
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

    #endregion

    #region GetSignature

    [Fact]
    public void GetSignature_CreatesValidSignature()
    {
        // Arrange
        var plaintext = ConvertibleString.FromUtf8("Test");

        // Act
        var signature = _signatureHelper.CreateSignature(_validPrivateKey, plaintext);

        // Assert
        var isValid = _signatureHelper.VerifySignature(plaintext, signature, _validPublicKey);
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

    #region IsValidPublicKey

    [Fact]
    public void IsValidPublicKey_ReturnsTrue_WhenPublicKeyIsValid()
    {
        var _ = _signatureHelper.VerifySignature(ConvertibleString.FromUtf8("Test"),
            ConvertibleString.FromBase64(""),
            ConvertibleString.FromBase64("Y8ZG4ikthK/Tvql7MwM9blvifnneN0nw5qQTVI7gvEw="));

        // Act
        var isValid = _signatureHelper.IsValidPublicKey(_validPublicKey);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValidPublicKey_ReturnsFalse_WhenPublicKeyIsInvalid()
    {
        // Arrange
        var invalidPublicKey = ConvertibleString.FromBase64(""); // modified first char

        // Act
        var isValid = _signatureHelper.IsValidPublicKey(invalidPublicKey);

        // Assert
        Assert.False(isValid);
    }

    #endregion

    #region Test Methods

    [Fact]
    public void TestImport()
    {
        var key = Key.Create(new Ed25519(),
            new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
        var _ = ConvertibleString.FromByteArray(key.PublicKey.Export(KeyBlobFormat.RawPublicKey));
        var privateKey = ConvertibleString.FromByteArray(key.Export(KeyBlobFormat.RawPrivateKey));

        _signatureHelper.CreateSignature(privateKey, ConvertibleString.FromUtf8("Test"));
        try
        {
            Key.Import(SignatureAlgorithm.Ed25519, _validPrivateKey.BytesRepresentation,
                KeyBlobFormat.RawPrivateKey);
        }
        catch (Exception)
        {
        }

        try
        {
            Key.Import(SignatureAlgorithm.Ed25519, _validPrivateKey.BytesRepresentation,
                KeyBlobFormat.PkixPrivateKey);
        }
        catch (Exception)
        {
        }

        try
        {
            Key.Import(SignatureAlgorithm.Ed25519, _validPrivateKey.BytesRepresentation,
                KeyBlobFormat.NSecPrivateKey);
        }
        catch (Exception)
        {
        }

        try
        {
            Key.Import(SignatureAlgorithm.Ed25519, _validPrivateKey.BytesRepresentation,
                KeyBlobFormat.PkixPrivateKeyText);
        }
        catch (Exception)
        {
        }
    }

    [Fact]
    public void GenerateSampleKeyPair()
    {
        var privateKey = Key.Create(SignatureAlgorithm.Ed25519, new KeyCreationParameters
        {
            ExportPolicy = KeyExportPolicies.AllowPlaintextExport
        });

        var publicKeyBytes = privateKey.Export(KeyBlobFormat.NSecPublicKey);
        _ = ConvertibleString.FromByteArray(publicKeyBytes).Base64Representation;

        var privateKeyBytes = privateKey.Export(KeyBlobFormat.NSecPrivateKey);
        _ = ConvertibleString.FromByteArray(privateKeyBytes).Base64Representation;
    }

    [Fact]
    public void CreateSignature()
    {
        _signatureHelper.CreateSignature(_validPrivateKey, ConvertibleString.FromUtf8("Test"));
    }

    #endregion

    #endregion
}
