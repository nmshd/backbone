namespace Backbone.Crypto.Abstractions;

public interface ISignatureHelper
{
    KeyPair CreateKeyPair();

    bool VerifySignature(ConvertibleString message, ConvertibleString signature, ConvertibleString publicKey);
    ConvertibleString CreateSignature(ConvertibleString privateKey, ConvertibleString message);
    bool IsValidPublicKey(ConvertibleString publicKey);
    bool IsValidPrivateKey(ConvertibleString privateKey);
}
