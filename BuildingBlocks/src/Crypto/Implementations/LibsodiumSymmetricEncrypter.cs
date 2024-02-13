using System.Text;
using System.Text.Json;
using Backbone.Crypto.Abstractions;
using Backbone.Tooling.JsonConverters;
using Sodium;

// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006

namespace Backbone.Crypto.Implementations;
public class LibsodiumSymmetricEncrypter : ISymmetricEncrypter
{
    public static byte[] DecryptXChaCha20Poly1305(byte[] body, byte[] key)
    {
        var bodyString = Encoding.UTF8.GetString(body);
        var deserializedBody = JsonSerializer.Deserialize<CryptoCipher>(bodyString,
            new JsonSerializerOptions { Converters = { new UrlSafeBase64ToByteArrayJsonConverter() } }) ??
            throw new InvalidOperationException("Decryption failed.");

        var cipherText = deserializedBody.cph.Replace('-', '+').Replace('_', '/') + "=";

        var payload = Convert.FromBase64String(cipherText);
        var nonce = Convert.FromBase64String(deserializedBody.nnc);

        return DecryptXChaCha20Poly1305(payload, nonce, key);
    }

    public static byte[] DecryptXChaCha20Poly1305(byte[] payload, byte[] nonce, byte[] key)
    {
        if (nonce.Length != 24)
            throw new ArgumentException("Nonce must be 24 bytes long for XChaCha20-Poly1305.", nameof(nonce));
        if (key.Length != 32)
            throw new ArgumentException("Key must be 32 bytes long for XChaCha20-Poly1305.", nameof(key));

        try
        {
            var decryptedData = SecretAeadXChaCha20Poly1305.Decrypt(payload, nonce, key);
            return decryptedData;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Decryption failed.", ex);
        }
    }

    private class CryptoCipher
    {
        /**
         * Algorithm (`alg`) and `@Type` fields are omitted due to not being used in the code.
         */
        public required string cph { get; init; }
        public required string nnc { get; init; }
    }

    private class Secret
    {
        /**
         * Algorithm (`alg`) field is omitted due to not being used in the code.
         */
        public required string Key { get; init; }
    }

    public ConvertibleString Decrypt(ConvertibleString encryptedMessage, ConvertibleString key)
    {
        ArgumentNullException.ThrowIfNull(encryptedMessage);
        ArgumentNullException.ThrowIfNull(key);

        var body = encryptedMessage.BytesRepresentation;
        var serializedSecret = key.BytesRepresentation;

        try
        {
            var decryptedBytes = DecryptXChaCha20Poly1305(body, serializedSecret);
            return ConvertibleString.FromByteArray(decryptedBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Decryption failed.", ex);
        }
    }

    public ConvertibleString Encrypt(ConvertibleString plaintext, ConvertibleString key)
    {
        throw new NotImplementedException();
    }
}
