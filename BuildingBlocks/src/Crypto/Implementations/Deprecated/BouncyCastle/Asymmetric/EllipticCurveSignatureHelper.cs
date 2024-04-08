using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations.Deprecated.BouncyCastle.ExtensionMethods;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Backbone.Crypto.Implementations.Deprecated.BouncyCastle.Asymmetric;

public class EllipticCurveSignatureHelper : ISignatureHelper
{
    private readonly string _signingAlgorithm;

    private EllipticCurveSignatureHelper(string signingAlgorithm)
    {
        _signingAlgorithm = signingAlgorithm;
    }

    public KeyPair CreateKeyPair()
    {
        throw new NotImplementedException();
    }

    public bool VerifySignature(ConvertibleString message, ConvertibleString signature, ConvertibleString publicKey)
    {
        var publicKeyParameters = CreatePublicKeyParameters(publicKey);

        var signer = SignerUtilities.GetSigner(_signingAlgorithm);

        signer.InitForValidation(publicKeyParameters);
        signer.BlockUpdate(message.BytesRepresentation);

        var isValid = signer.VerifySignature(signature.BytesRepresentation);
        return isValid;
    }

    public bool IsValidPublicKey(ConvertibleString publicKey)
    {
        try
        {
            CreatePublicKeyParameters(publicKey);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public ConvertibleString CreateSignature(ConvertibleString privateKey, ConvertibleString message)
    {
        var privateKeyParameters = CreatePrivateKeyParameters(privateKey);

        var signer = SignerUtilities.GetSigner(_signingAlgorithm);

        signer.InitForSigning(privateKeyParameters);
        signer.BlockUpdate(message.BytesRepresentation);

        var signature = signer.GenerateSignature();
        return ConvertibleString.FromByteArray(signature);
    }

    public bool IsValidPrivateKey(ConvertibleString privateKey)
    {
        throw new NotImplementedException();
    }

    private ECPrivateKeyParameters CreatePrivateKeyParameters(ConvertibleString privateKey)
    {
        try
        {
            var privateKeyParameters =
                (ECPrivateKeyParameters)PrivateKeyFactory.CreateKey(privateKey.BytesRepresentation);
            return privateKeyParameters;
        }
        catch (Exception ex) when (ex is ArgumentException || ex is IOException)
        {
            throw new ArgumentException("Private Key is invalid.", nameof(privateKey), ex);
        }
    }

    private ECPublicKeyParameters CreatePublicKeyParameters(ConvertibleString publicKey)
    {
        try
        {
            var publicKeyParameters =
                (ECPublicKeyParameters)PublicKeyFactory.CreateKey(publicKey.BytesRepresentation);
            return publicKeyParameters;
        }
        catch (Exception ex) when (ex is ArgumentException || ex is IOException)
        {
            throw new ArgumentException("Public Key is invalid.", nameof(publicKey), ex);
        }
    }

    public static EllipticCurveSignatureHelper CreateSha512WithEcdsa()
    {
        return new EllipticCurveSignatureHelper("SHA-512withECDSA");
    }
}
