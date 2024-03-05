namespace Backbone.Crypto;

public class KeyPair
{
    public KeyPair(ConvertibleString publicKey, ConvertibleString privateKey)
    {
        PublicKey = publicKey;
        PrivateKey = privateKey;
    }

    public ConvertibleString PublicKey { get; }
    public ConvertibleString PrivateKey { get; }
}
