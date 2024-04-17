// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006

namespace Backbone.ConsumerApi.Sdk.Endpoints.Common.Crypto;

public class CryptoSignaturePublicKey
{
    public required CryptoExchangeAlgorithm alg;
    public required string pub;
}
