using Backbone.AdminApi.Sdk.Endpoints.Challenges.Types;
using Backbone.AdminApi.Sdk.Endpoints.Common.Crypto;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.Crypto;
using Newtonsoft.Json;
using SignatureHelper = Backbone.Crypto.Implementations.SignatureHelper;

namespace Backbone.AdminApi.Sdk.Services;

public class AccountController(Client client)
{
    public async Task<ApiResponse<CreateIdentityResponse>?> CreateIdentity(string clientId, string clientSecret)
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();

        var identityKeyPair = signatureHelper.CreateKeyPair();

        var challenge = await client.Challenges.CreateChallenge();
        if (challenge.Result?.Id is null)
            return null;

        var serializedChallenge = JsonConvert.SerializeObject(challenge.Result);

        var challengeSignature = signatureHelper.CreateSignature(identityKeyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));
        var signedChallenge = new SignedChallenge(serializedChallenge, challengeSignature);

        var createIdentityPayload = new CreateIdentityRequest
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            IdentityVersion = 1,
            SignedChallenge = signedChallenge,
            IdentityPublicKey = ConvertibleString.FromUtf8(JsonConvert.SerializeObject(new CryptoSignaturePublicKey
            {
                alg = CryptoExchangeAlgorithm.ECDH_X25519,
                pub = identityKeyPair.PublicKey.Base64Representation
            })).Base64Representation,
            DevicePassword = PasswordHelper.GeneratePassword(45, 50)
        };

        return await client.Identities.CreateIdentity(createIdentityPayload);
    }
}
