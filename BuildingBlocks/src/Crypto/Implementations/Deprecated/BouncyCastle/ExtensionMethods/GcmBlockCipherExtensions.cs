using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Backbone.Crypto.Implementations.Deprecated.BouncyCastle.ExtensionMethods;

public static class GcmBlockCipherExtensions
{
    public static byte[] Decrypt(this GcmBlockCipher cipher, byte[] cipherText)
    {
        var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];
        var len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
        cipher.DoFinal(plainText, len);
        return plainText;
    }

    public static byte[] Encrypt(this GcmBlockCipher cipher, byte[] messageToEncrypt)
    {
        var cipherText = new byte[cipher.GetOutputSize(messageToEncrypt.Length)];
        var len = cipher.ProcessBytes(messageToEncrypt, 0, messageToEncrypt.Length, cipherText, 0);
        cipher.DoFinal(cipherText, len);
        return cipherText;
    }

    public static void InitForEncryption(this GcmBlockCipher cipher, AeadParameters parameters)
    {
        cipher.Init(true, parameters);
    }

    public static void InitForDecryption(this GcmBlockCipher cipher, AeadParameters parameters)
    {
        cipher.Init(false, parameters);
    }
}
