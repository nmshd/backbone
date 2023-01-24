using Enmeshed.Crypto.Abstractions;
using NSec.Cryptography;

namespace Enmeshed.Crypto.Implementations
{
    public class KeyGenerator : IKeyGenerator
    {
        private readonly KeyBlobFormat _keyFormat;

        public KeyGenerator(KeyBlobFormat keyFormat)
        {
            _keyFormat = keyFormat;
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
}