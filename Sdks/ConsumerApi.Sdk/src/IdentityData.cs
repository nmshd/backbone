using Backbone.Crypto;

namespace Backbone.ConsumerApi.Sdk;

public record IdentityData
{
    public required string Address;
    public KeyPair? KeyPair;
}
