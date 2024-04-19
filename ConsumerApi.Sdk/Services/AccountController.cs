using Backbone.ConsumerApi.Sdk.Endpoints.Common.Crypto;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Sdk.Services;
public class AccountController(Client client)
{
    /// <summary>
    /// A list of all received ResponseRepresentations for identities.
    /// </summary>
    public List<ResponseRepresentation> ReceivedResponseRepresentations = [];

    public async Task<ResponseRepresentation?> CreateIdentity(string clientId, string clientSecret)
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();

        var identityKeyPair = signatureHelper.CreateKeyPair();

        var challenge = await client.Challenges.CreateChallengeUnauthenticated();
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

        var createIdentityResponse = (await client.Identities.CreateIdentity(createIdentityPayload)).Result;

        if (createIdentityResponse is null)
            return null;

        var responseRepresentation = new ResponseRepresentation
        {
            Challenge = challenge.Result.Id,
            PrivateKey = identityKeyPair.PrivateKey.Base64Representation,
            PublicKey = identityKeyPair.PublicKey.Base64Representation,
            DevicePassword = createIdentityPayload.DevicePassword,
            SignedChallenge = challengeSignature.Base64Representation,
            CreatedAt = createIdentityResponse.CreatedAt,
            Address = createIdentityResponse.Address,
            DeviceId = createIdentityResponse.Device.Id,
            DeviceUsername = createIdentityResponse.Device.Username
        };

        ReceivedResponseRepresentations.Add(responseRepresentation);

        return responseRepresentation;
    }
}

public class ResponseRepresentation
{
    public required string PrivateKey;
    public required string PublicKey;
    public required string DevicePassword;
    public required string Challenge;
    public required string SignedChallenge;
    public required DateTime CreatedAt;
    public required string Address;
    public required string DeviceId;
    public required string DeviceUsername;
}
