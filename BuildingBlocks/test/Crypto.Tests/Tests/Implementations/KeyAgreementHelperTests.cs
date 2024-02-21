using Backbone.Crypto.Implementations;
using FluentAssertions;
using Xunit;

namespace Backbone.Crypto.Tests.Tests.Implementations;

public class KeyAgreementHelperTests
{
    private readonly KeyAgreementHelper _keyAgreementHelper;

    [Fact]
    public void IsValidPublicKey_ShouldReturnTrue_WhenPublicKeyIsValid()
    {
        var isValid = _keyAgreementHelper.IsValidPublicKey(_validPublicKey);
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValidPublicKey_ShouldReturnFalse_WhenPublicKeyIsInvalid()
    {
        var invalidPublicKey = ConvertibleString.FromBase64("DFN0");
        var isValid = _keyAgreementHelper.IsValidPublicKey(invalidPublicKey);
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsValidPrivateKey_ShouldReturnTrue_WhenPrivateKeyIsValid()
    {
        var isValid = _keyAgreementHelper.IsValidPublicKey(_validPrivateKey);
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValidPrivateKey_ShouldReturnFalse_WhenPrivateKeyIsInvalid()
    {
        var invalidPublicKey = ConvertibleString.FromBase64("k7oq");
        var isValid = _keyAgreementHelper.IsValidPublicKey(invalidPublicKey);
        isValid.Should().BeFalse();
    }

    #region Test Data

    private readonly ConvertibleString _validPublicKey =
        ConvertibleString.FromBase64("DFN0+ITWU8c45PDkp9vEj+OILm/ej3Egy/Nunn8KFX0=");

    private readonly ConvertibleString _validPrivateKey =
        ConvertibleString.FromBase64("k7oqkrRtS9yLA532owbpRcavM8qF7pEr60uhI9dn6lo=");

    #endregion

    #region Setup and Teardown

    public KeyAgreementHelperTests()
    {
        _keyAgreementHelper = KeyAgreementHelper.CreateX25519WithRawKeyFormat();
    }

    #endregion
}
