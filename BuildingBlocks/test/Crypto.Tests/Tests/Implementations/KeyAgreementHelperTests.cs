using Backbone.Crypto.Implementations;

namespace Backbone.Crypto.Tests.Tests.Implementations;

public class KeyAgreementHelperTests : AbstractTestsBase
{
    private readonly KeyAgreementHelper _keyAgreementHelper;

    [Fact]
    public void IsValidPublicKey_ShouldReturnTrue_WhenPublicKeyIsValid()
    {
        var isValid = _keyAgreementHelper.IsValidPublicKey(_validPublicKey);
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void IsValidPublicKey_ShouldReturnFalse_WhenPublicKeyIsInvalid()
    {
        var invalidPublicKey = ConvertibleString.FromBase64("DFN0");
        var isValid = _keyAgreementHelper.IsValidPublicKey(invalidPublicKey);
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void IsValidPrivateKey_ShouldReturnTrue_WhenPrivateKeyIsValid()
    {
        var isValid = _keyAgreementHelper.IsValidPublicKey(_validPrivateKey);
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void IsValidPrivateKey_ShouldReturnFalse_WhenPrivateKeyIsInvalid()
    {
        var invalidPublicKey = ConvertibleString.FromBase64("k7oq");
        var isValid = _keyAgreementHelper.IsValidPublicKey(invalidPublicKey);
        isValid.ShouldBeFalse();
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
