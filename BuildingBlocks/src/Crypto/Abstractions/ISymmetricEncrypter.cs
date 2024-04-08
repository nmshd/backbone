namespace Backbone.Crypto.Abstractions;

public interface ISymmetricEncrypter
{
    ConvertibleString Decrypt(ConvertibleString encryptedMessage, ConvertibleString key);
    ConvertibleString Encrypt(ConvertibleString plaintext, ConvertibleString key);
}
