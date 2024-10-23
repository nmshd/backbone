// ReSharper disable InconsistentNaming

#pragma warning disable IDE1006

namespace Backbone.BuildingBlocks.SDK.Crypto;

public class CryptoSignaturePublicKey
{
    public required CryptoExchangeAlgorithm alg { get; set; }
    public required string pub { get; set; }
}
