namespace Backbone.Crypto.Abstractions;

public interface IKeyAgreementHelper
{
    bool IsValidPublicKey(ConvertibleString publicKey);
    bool IsValidPrivateKey(ConvertibleString privateKey);
    KeyPair CreateKeyPair();
}
