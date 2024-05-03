// ReSharper disable InconsistentNaming

#pragma warning disable IDE1006

namespace Backbone.BuildingBlocks.SDK.Crypto;

public class CryptoSignatureSignedChallenge
{
    public required CryptoHashAlgorithm alg;
    public required byte[] sig;
}
