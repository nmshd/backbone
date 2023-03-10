using Enmeshed.Crypto.Abstractions;
using NSec.Cryptography;

namespace Enmeshed.Crypto.Implementations;

public class SignatureHelper : ISignatureHelper
{
    private readonly SignatureAlgorithm _signatureAlgorithm;
    private readonly KeyBlobFormat _privateKeyFormat;
    private readonly KeyBlobFormat _publicKeyFormat;

    public SignatureHelper(SignatureAlgorithm signatureAlgorithm, KeyBlobFormat privateKeyFormat,
        KeyBlobFormat publicKeyFormat)
    {
        _signatureAlgorithm = signatureAlgorithm;
        _privateKeyFormat = privateKeyFormat;
        _publicKeyFormat = publicKeyFormat;
    }

    public KeyPair CreateKeyPair()
    {
        using var key = Key.Create(_signatureAlgorithm,
            new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });

        var publicKey = key.PublicKey.Export(_publicKeyFormat);
        var privateKey = key.Export(_privateKeyFormat);

        return new KeyPair(ConvertibleString.FromByteArray(publicKey), ConvertibleString.FromByteArray(privateKey));
    }

    public bool VerifySignature(ConvertibleString message, ConvertibleString signature, ConvertibleString publicKey)
    {
        var key = ImportPublicKey(publicKey);
        var isValid = _signatureAlgorithm.Verify(key, message.BytesRepresentation, signature.BytesRepresentation);
        return isValid;
    }

    private PublicKey ImportPublicKey(ConvertibleString publicKey)
    {
        try
        {
            var key = PublicKey.Import(_signatureAlgorithm, publicKey.BytesRepresentation, _publicKeyFormat);
            return key;
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Public Key is invalid.", nameof(publicKey), ex);
        }
    }

    public bool IsValidPublicKey(ConvertibleString publicKey)
    {
        return PublicKey.TryImport(_signatureAlgorithm, publicKey.BytesRepresentation, _publicKeyFormat, out _);
    }

    public ConvertibleString GetSignature(ConvertibleString privateKey, ConvertibleString message)
    {
        var key = ImportPrivateKey(privateKey);
        var signature = _signatureAlgorithm.Sign(key, message.BytesRepresentation);
        return ConvertibleString.FromByteArray(signature);
    }

    private Key ImportPrivateKey(ConvertibleString privateKey)
    {
        try
        {
            var k = Key.Create(_signatureAlgorithm,
                new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
            var pk = ConvertibleString.FromByteArray(k.Export(KeyBlobFormat.RawPrivateKey)).Base64Representation;
            var pubk = ConvertibleString.FromByteArray(k.PublicKey.Export(KeyBlobFormat.RawPublicKey))
                .Base64Representation;

            var key = Key.Import(_signatureAlgorithm, privateKey.BytesRepresentation, _privateKeyFormat);
            return key;
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Private Key is invalid.", nameof(privateKey), ex);
        }
    }

    public bool IsValidPrivateKey(ConvertibleString privateKey)
    {
        return Key.TryImport(_signatureAlgorithm, privateKey.BytesRepresentation, _privateKeyFormat, out _);
    }

    public static SignatureHelper CreateEd25519WithRawKeyFormat()
    {
        Algorithm x = new X25519();
        return new SignatureHelper(SignatureAlgorithm.Ed25519, KeyBlobFormat.RawPrivateKey,
            KeyBlobFormat.RawPublicKey);
    }
}