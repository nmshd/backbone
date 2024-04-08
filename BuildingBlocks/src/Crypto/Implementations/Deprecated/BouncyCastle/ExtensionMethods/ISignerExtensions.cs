using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace Backbone.Crypto.Implementations.Deprecated.BouncyCastle.ExtensionMethods;

// ReSharper disable once InconsistentNaming
internal static class ISignerExtensions
{
    public static void InitForSigning(this ISigner signer, ECPrivateKeyParameters publicKeyParameters)
    {
        signer.Init(true, publicKeyParameters);
    }

    public static void InitForValidation(this ISigner signer, ECPublicKeyParameters publicKeyParameters)
    {
        signer.Init(false, publicKeyParameters);
    }

    public static void BlockUpdate(this ISigner signer, byte[] input)
    {
        signer.BlockUpdate(input, 0, input.Length);
    }
}
