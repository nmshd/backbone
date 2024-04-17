// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Backbone.ConsumerApi.Sdk.Endpoints.Common;

#pragma warning disable IDE1006
public class CryptoSignaturePublicKey
{
    public required CryptoExchangeAlgorithm alg;
    public required string pub;
}

public class CryptoSignatureSignedChallenge
{
    public required CryptoHashAlgorithm alg;
    public required byte[] sig;
}
#pragma warning restore IDE1006

public enum CryptoExchangeAlgorithm
{
    ECDH_P256 = 1,
    ECDH_P521 = 2,
    ECDH_X25519 = 3
}

public enum CryptoHashAlgorithm
{
    // SHA256 Hash Algorithm with a hash of 32 bytes
    SHA256 = 1,
    // SHA512 Hash Algorithm with a hash of 64 bytes
    SHA512 = 2,
    BLAKE2B = 3
}
