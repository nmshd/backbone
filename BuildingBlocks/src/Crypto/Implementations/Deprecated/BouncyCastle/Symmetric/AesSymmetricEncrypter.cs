using Backbone.Crypto.Abstractions;
using Backbone.Crypto.ExtensionMethods;
using Backbone.Crypto.Implementations.Deprecated.BouncyCastle.ExtensionMethods;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Backbone.Crypto.Implementations.Deprecated.BouncyCastle.Symmetric;

public class AesSymmetricEncrypter : ISymmetricEncrypter
{
    private readonly int _ivSize;
    private readonly int _macSize;

    private readonly SecureRandom _randomNumberGenerator;

    public AesSymmetricEncrypter(int macBitSize, int ivBitSize)
    {
        _randomNumberGenerator = new SecureRandom();

        _macSize = macBitSize;
        _ivSize = ivBitSize;
    }

    public ConvertibleString Decrypt(ConvertibleString encryptedMessage, ConvertibleString key)
    {
        if (encryptedMessage.IsEmpty())
        {
            throw new ArgumentException("Encrypted Message Required", nameof(encryptedMessage));
        }

        var iv = encryptedMessage.BytesRepresentation.Take(_ivSize / 8).ToArray();
        var cipherText = encryptedMessage.BytesRepresentation.TakeFrom(iv.Length).ToArray();
        var cipher = CreateDecryptionCipher(iv, key);
        var plainText = cipher.Decrypt(cipherText);

        return ConvertibleString.FromByteArray(plainText);
    }

    public ConvertibleString Encrypt(ConvertibleString plaintext, ConvertibleString key)
    {
        if (plaintext.IsEmpty())
        {
            throw new ArgumentException("Encrypted Message Required", nameof(plaintext));
        }

        var iv = GenerateIv();
        var cipher = CreateEncryptionCipher(iv, key);
        var cipherText = cipher.Encrypt(plaintext.BytesRepresentation);
        var encryptedMessage = iv.Concat(cipherText).ToArray();

        return ConvertibleString.FromByteArray(encryptedMessage);
    }

    private byte[] GenerateIv()
    {
        var iv = new byte[_ivSize / 8];
        _randomNumberGenerator.NextBytes(iv, 0, iv.Length);
        return iv;
    }

    private GcmBlockCipher CreateDecryptionCipher(byte[] iv, ConvertibleString key)
    {
        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(key.BytesRepresentation), _macSize, iv);

        try
        {
            cipher.InitForDecryption(parameters);
            return cipher;
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException(ex.Message, nameof(key), ex);
        }
    }

    private GcmBlockCipher CreateEncryptionCipher(byte[] iv, ConvertibleString key)
    {
        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(key.BytesRepresentation), _macSize, iv);
        try
        {
            cipher.InitForEncryption(parameters);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException(ex.Message, nameof(key), ex);
        }

        return cipher;
    }

    public static AesSymmetricEncrypter CreateWith96BitIv128BitMac()
    {
        return new AesSymmetricEncrypter(128, 96);
    }
}
