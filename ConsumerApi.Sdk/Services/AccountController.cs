using Backbone.Crypto.Implementations;

namespace Backbone.ConsumerApi.Sdk.Services;
public class AccountController(Client client)
{
    public Client Client = client;

    public async Task<Tuple<string, string>> CreateIdentity()
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();

        var identityKeyPair = signatureHelper.CreateKeyPair();
        var devicePwd1 = PasswordHelper.GeneratePassword(45, 50);
        var deviceKeyPair = signatureHelper.CreateKeyPair();
        
        //var privBaseShared = 
        //var provBaseDevice = 
        //var privSync = 

        //var challenge = await client.Challenges.CreateChallengeUnauthenticated();

        return new(identityKeyPair.PublicKey.Base64Representation, identityKeyPair.PrivateKey.Base64Representation);        
    }
}
