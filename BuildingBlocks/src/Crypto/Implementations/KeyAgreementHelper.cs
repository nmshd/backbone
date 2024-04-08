using Backbone.Crypto.Abstractions;
using NSec.Cryptography;

namespace Backbone.Crypto.Implementations;

public class KeyAgreementHelper : IKeyAgreementHelper
{
    private readonly KeyAgreementAlgorithm _keyAgreementAlgorithm;
    private readonly KeyBlobFormat _privateKeyFormat;
    private readonly KeyBlobFormat _publicKeyFormat;

    private KeyAgreementHelper(KeyAgreementAlgorithm keyAgreementAlgorithm, KeyBlobFormat privateKeyFormat,
        KeyBlobFormat publicKeyFormat)
    {
        _keyAgreementAlgorithm = keyAgreementAlgorithm;
        _privateKeyFormat = privateKeyFormat;
        _publicKeyFormat = publicKeyFormat;
    }

    public bool IsValidPublicKey(ConvertibleString publicKey)
    {
        return PublicKey.TryImport(_keyAgreementAlgorithm, publicKey.BytesRepresentation, _publicKeyFormat, out _);
    }

    public bool IsValidPrivateKey(ConvertibleString privateKey)
    {
        return PublicKey.TryImport(_keyAgreementAlgorithm, privateKey.BytesRepresentation, _publicKeyFormat, out _);
    }

    public KeyPair CreateKeyPair()
    {
        using var key = Key.Create(_keyAgreementAlgorithm,
            new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });

        var publicKey = key.PublicKey.Export(_publicKeyFormat);
        var privateKey = key.Export(_privateKeyFormat);

        return new KeyPair(ConvertibleString.FromByteArray(publicKey), ConvertibleString.FromByteArray(privateKey));
    }

    public static KeyAgreementHelper CreateX25519WithRawKeyFormat()
    {
        return new KeyAgreementHelper(KeyAgreementAlgorithm.X25519, KeyBlobFormat.RawPrivateKey,
            KeyBlobFormat.RawPublicKey);
    }
}
