namespace Backbone.Crypto.Abstractions;

public interface IKeyGenerator
{
    ConvertibleString DeriveSymmetricKeyWithEcdh(ConvertibleString privateKey, ConvertibleString publicKey,
        int keyLengthInBits);

    ConvertibleString GenerateSymmetricKey(int keySize);
}
