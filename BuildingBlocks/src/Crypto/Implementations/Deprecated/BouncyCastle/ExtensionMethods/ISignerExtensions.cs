using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace Backbone.Crypto.Implementations.Deprecated.BouncyCastle.ExtensionMethods;

// ReSharper disable once InconsistentNaming
internal static class ISignerExtensions
{
    extension(ISigner signer)
    {
        public void InitForSigning(ECPrivateKeyParameters publicKeyParameters)
        {
            signer.Init(true, publicKeyParameters);
        }

        public void InitForValidation(ECPublicKeyParameters publicKeyParameters)
        {
            signer.Init(false, publicKeyParameters);
        }

        public void BlockUpdate(byte[] input)
        {
            signer.BlockUpdate(input, 0, input.Length);
        }
    }
}
