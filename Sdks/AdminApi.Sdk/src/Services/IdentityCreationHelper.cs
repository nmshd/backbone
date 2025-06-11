using System.Text.Json;
using Backbone.AdminApi.Sdk.Endpoints.Challenges.Types;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Responses;
using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.Crypto;
using SignatureHelper = Backbone.Crypto.Implementations.SignatureHelper;

namespace Backbone.AdminApi.Sdk.Services;

public static class IdentityCreationHelper
{
    public const string DEVICE_PASSWORD = "some-device-password";
    public const string TEST_CLIENT_ID = "test";
    public const string DEFAULT_DEVICE_COMMUNICATION_LANGUAGE = "en";

    public static async Task<ApiResponse<CreateIdentityResponse>> CreateIdentity(Client client)
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();

        var identityKeyPair = signatureHelper.CreateKeyPair();

        var challenge = await client.Challenges.CreateChallenge();
        var serializedChallenge = JsonSerializer.Serialize(challenge.Result);

        var challengeSignature = signatureHelper.CreateSignature(identityKeyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));
        var signedChallenge = new SignedChallenge(serializedChallenge, challengeSignature);

        var createIdentityPayload = new CreateIdentityRequest
        {
            ClientId = TEST_CLIENT_ID,
            IdentityVersion = 1,
            SignedChallenge = signedChallenge,
            DeviceCommunicationLanguage = DEFAULT_DEVICE_COMMUNICATION_LANGUAGE,
            IdentityPublicKey = ConvertibleString.FromUtf8(JsonSerializer.Serialize(new CryptoSignaturePublicKey
            {
                alg = CryptoExchangeAlgorithm.ECDH_X25519,
                pub = identityKeyPair.PublicKey.Base64Representation
            })).BytesRepresentation,
            DevicePassword = DEVICE_PASSWORD
        };

        return await client.Identities.CreateIdentity(createIdentityPayload);
    }
}
