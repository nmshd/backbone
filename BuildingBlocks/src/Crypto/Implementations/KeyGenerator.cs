using Backbone.Crypto.Abstractions;
using NSec.Cryptography;

namespace Backbone.Crypto.Implementations;

public class KeyGenerator : IKeyGenerator
{
    public KeyGenerator()
    {
    }

    public ConvertibleString DeriveSymmetricKeyWithEcdh(ConvertibleString privateKey, ConvertibleString publicKey,
        int keyLengthInBits)
    {
        throw new NotImplementedException();
    }

    public ConvertibleString GenerateSymmetricKey(int keySize)
    {
        throw new NotImplementedException();

        // TODO: how to set key size?
        // using var key = Key.Create(AeadAlgorithm.Aes256Gcm, new KeyCreationParameters
        // {
        //     ExportPolicy = KeyExportPolicies.AllowPlaintextExport
        // });
        //
        // return ConvertibleString.FromByteArray(key.Export(_keyFormat));
    }
}
