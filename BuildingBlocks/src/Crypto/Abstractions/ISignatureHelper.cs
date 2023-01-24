namespace Enmeshed.Crypto.Abstractions
{
    public interface ISignatureHelper
    {
        KeyPair CreateKeyPair();

        bool VerifySignature(ConvertibleString message, ConvertibleString signature, ConvertibleString publicKey);
        ConvertibleString GetSignature(ConvertibleString privateKey, ConvertibleString message);

        bool IsValidPublicKey(ConvertibleString publicKey);
        bool IsValidPrivateKey(ConvertibleString privateKey);
    }
}