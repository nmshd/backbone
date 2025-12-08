using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Backbone.Crypto.Implementations.Deprecated.BouncyCastle.ExtensionMethods;

public static class GcmBlockCipherExtensions
{
    extension(GcmBlockCipher cipher)
    {
        public byte[] Decrypt(byte[] cipherText)
        {
            var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];
            var len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
            cipher.DoFinal(plainText, len);
            return plainText;
        }

        public byte[] Encrypt(byte[] messageToEncrypt)
        {
            var cipherText = new byte[cipher.GetOutputSize(messageToEncrypt.Length)];
            var len = cipher.ProcessBytes(messageToEncrypt, 0, messageToEncrypt.Length, cipherText, 0);
            cipher.DoFinal(cipherText, len);
            return cipherText;
        }

        public void InitForEncryption(AeadParameters parameters)
        {
            cipher.Init(true, parameters);
        }

        public void InitForDecryption(AeadParameters parameters)
        {
            cipher.Init(false, parameters);
        }
    }
}
