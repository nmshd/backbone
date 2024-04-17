// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Backbone.ConsumerApi.Sdk.Endpoints.Common.Crypto;

public enum CryptoHashAlgorithm
{
    // SHA256 Hash Algorithm with a hash of 32 bytes
    SHA256 = 1,
    // SHA512 Hash Algorithm with a hash of 64 bytes
    SHA512 = 2,
    BLAKE2B = 3
}
