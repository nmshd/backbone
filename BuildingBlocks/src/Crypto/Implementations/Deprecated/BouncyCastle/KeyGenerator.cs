using Backbone.Crypto.Abstractions;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Backbone.Crypto.Implementations.Deprecated.BouncyCastle;

public class KeyGenerator : IKeyGenerator
{
    private readonly SecureRandom _randomNumberGenerator;

    public KeyGenerator()
    {
        _randomNumberGenerator = new SecureRandom();
    }

    public ConvertibleString DeriveSymmetricKeyWithEcdh(ConvertibleString privateKey, ConvertibleString publicKey,
        int keyLengthInBits)
    {
        var privateKeyParameters =
            (ECPrivateKeyParameters)PrivateKeyFactory.CreateKey(privateKey.BytesRepresentation);
        var publicKeyParameters = (ECPublicKeyParameters)PublicKeyFactory.CreateKey(publicKey.BytesRepresentation);

        var keyAgreement = AgreementUtilities.GetBasicAgreement("ECDH");
        keyAgreement.Init(privateKeyParameters);
        var agreement = keyAgreement.CalculateAgreement(publicKeyParameters);

        var sharedKey = new KeyParameter(agreement.ToByteArrayUnsigned().Take(keyLengthInBits / 8).ToArray());
        return ConvertibleString.FromByteArray(sharedKey.GetKey());
    }

    public ConvertibleString GenerateSymmetricKey(int keySize)
    {
        var key = new byte[keySize / 8];
        _randomNumberGenerator.NextBytes(key);
        return ConvertibleString.FromByteArray(key);
    }
}
