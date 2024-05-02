using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges;
using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Files;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications;
using Backbone.ConsumerApi.Sdk.Endpoints.Quotas;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Sdk;

public class Client
{
    private readonly HttpClient _httpClient;
    private readonly KeyPair? _identityKeyPair;

    private Client(HttpClient httpClient, Configuration configuration, KeyPair? identityKeyPair = null)
    {
        var authenticator = new OAuthAuthenticator(configuration.Authentication, httpClient);
        var endpointClient = new EndpointClient(httpClient, authenticator, configuration.JsonSerializerOptions);

        Configuration = configuration;
        _httpClient = httpClient;
        _identityKeyPair = identityKeyPair;

        Challenges = new ChallengesEndpoint(endpointClient);
        Datawallet = new DatawalletEndpoint(endpointClient);
        Devices = new DevicesEndpoint(endpointClient);
        Files = new FilesEndpoint(endpointClient);
        Identities = new IdentitiesEndpoint(endpointClient);
        Messages = new MessagesEndpoint(endpointClient);
        PushNotifications = new PushNotificationsEndpoint(endpointClient);
        Quotas = new QuotasEndpoint(endpointClient);
        Relationships = new RelationshipsEndpoint(endpointClient);
        RelationshipTemplates = new RelationshipTemplatesEndpoint(endpointClient);
        SyncRuns = new SyncRunsEndpoint(endpointClient);
        Tokens = new TokensEndpoint(endpointClient);
    }

    public Configuration Configuration { get; }

    public ChallengesEndpoint Challenges { get; }
    public DatawalletEndpoint Datawallet { get; }
    public DevicesEndpoint Devices { get; }
    public FilesEndpoint Files { get; }
    public IdentitiesEndpoint Identities { get; }
    public MessagesEndpoint Messages { get; }
    public PushNotificationsEndpoint PushNotifications { get; }
    public QuotasEndpoint Quotas { get; }
    public RelationshipsEndpoint Relationships { get; }
    public RelationshipTemplatesEndpoint RelationshipTemplates { get; }
    public SyncRunsEndpoint SyncRuns { get; }
    public TokensEndpoint Tokens { get; }

    public static Client CreateUnauthenticated(string baseUrl, ClientCredentials clientCredentials)
    {
        return CreateUnauthenticated(new HttpClient { BaseAddress = new Uri(baseUrl) }, clientCredentials);
    }

    public static Client CreateUnauthenticated(HttpClient httpClient, ClientCredentials clientCredentials)
    {
        if (httpClient.BaseAddress == null)
            throw new Exception("The base address of the HttpClient must be set.");

        var configuration = new Configuration
        {
            Authentication = new Configuration.AuthenticationConfiguration
            {
                ClientId = clientCredentials.ClientId,
                ClientSecret = clientCredentials.ClientSecret,
                Username = "",
                Password = ""
            }
        };

        return new Client(httpClient, configuration);
    }

    public static async Task<Client> CreateForNewIdentity(string baseUrl, ClientCredentials clientCredentials, string password = "Password")
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

        return await CreateForNewIdentity(httpClient, clientCredentials);
    }

    public static async Task<Client> CreateForNewIdentity(HttpClient httpClient, ClientCredentials clientCredentials, string password = "Password")
    {
        if (httpClient.BaseAddress == null)
            throw new Exception("The base address of the HttpClient must be set.");

        var identity = await CreateIdentity(httpClient, clientCredentials, password);

        var configuration = new Configuration
        {
            Authentication = new Configuration.AuthenticationConfiguration
            {
                ClientId = clientCredentials.ClientId,
                ClientSecret = clientCredentials.ClientSecret,
                Username = identity.DeviceUsername,
                Password = identity.DevicePassword
            }
        };

        var client = new Client(httpClient, configuration, identity.KeyPair);

        return client;
    }

    private static async Task<ResponseRepresentation> CreateIdentity(HttpClient httpClient, ClientCredentials clientCredentials, string password)
    {
        var temporaryConfiguration = new Configuration
        {
            Authentication = new Configuration.AuthenticationConfiguration
            {
                ClientId = clientCredentials.ClientId,
                ClientSecret = clientCredentials.ClientSecret,
                Username = "",
                Password = ""
            }
        };

        var client = new Client(httpClient, temporaryConfiguration);

        var (keyPair, signedChallenge) = await CreateSignedChallenge(client);

        var createIdentityPayload = new CreateIdentityRequest
        {
            ClientId = client.Configuration.Authentication.ClientId,
            ClientSecret = client.Configuration.Authentication.ClientSecret,
            IdentityVersion = 1,
            SignedChallenge = signedChallenge,
            IdentityPublicKey = ConvertibleString.FromUtf8(JsonConvert.SerializeObject(new CryptoSignaturePublicKey
            {
                alg = CryptoExchangeAlgorithm.ECDH_X25519,
                pub = keyPair.PublicKey.Base64Representation
            })).BytesRepresentation,
            DevicePassword = password
        };

        var createIdentityResponse = await client.Identities.CreateIdentity(createIdentityPayload);

        if (createIdentityResponse.IsError)
            throw new Exception($"There was an error when creating the identity. The error code was '{createIdentityResponse.Error.Code}'. The message was '{createIdentityResponse.Error.Message}'.");

        var responseRepresentation = new ResponseRepresentation
        {
            KeyPair = keyPair,
            DevicePassword = createIdentityPayload.DevicePassword,
            CreatedAt = createIdentityResponse.Result.CreatedAt,
            Address = createIdentityResponse.Result.Address,
            DeviceId = createIdentityResponse.Result.Device.Id,
            DeviceUsername = createIdentityResponse.Result.Device.Username
        };

        return responseRepresentation;
    }

    public async Task<Client> OnboardNewDevice(string password = "Password")
    {
        var (_, signedChallenge) = await CreateSignedChallenge(this);

        var createDeviceResponse = await Devices.RegisterDevice(new RegisterDeviceRequest
        {
            DevicePassword = password,
            SignedChallenge = signedChallenge
        });

        if (createDeviceResponse.IsError)
            throw new Exception($"There was an error when creating the device. The error code was {createDeviceResponse.Error.Code}. The message was {createDeviceResponse.Error.Message}.");

        var configuration = new Configuration
        {
            Authentication = new Configuration.AuthenticationConfiguration
            {
                ClientId = Configuration.Authentication.ClientId,
                ClientSecret = Configuration.Authentication.ClientSecret,
                Username = createDeviceResponse.Result.Username,
                Password = password
            }
        };

        var client = new Client(_httpClient, configuration, _identityKeyPair);

        return client;
    }

    private static async Task<(KeyPair keyPair, SignedChallenge signedChallenge)> CreateSignedChallenge(Client client)
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();

        var keyPair = client._identityKeyPair ?? signatureHelper.CreateKeyPair();

        var createChallengeResponse = await client.Challenges.CreateChallengeUnauthenticated();
        if (createChallengeResponse.IsError)
            throw new Exception(
                $"There was an error when creating a challenge for the new identity. The error code was {createChallengeResponse.Error.Code}. The message was {createChallengeResponse.Error.Message}.");

        var serializedChallenge = JsonConvert.SerializeObject(createChallengeResponse.Result);

        var challengeSignature = signatureHelper.CreateSignature(keyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));
        var signedChallenge = new SignedChallenge
        {
            Challenge = serializedChallenge, Signature = ConvertibleString.FromUtf8(
                JsonConvert.SerializeObject(new CryptoSignatureSignedChallenge { alg = CryptoHashAlgorithm.SHA512, sig = challengeSignature.BytesRepresentation }
                )).BytesRepresentation
        };
        return (keyPair, signedChallenge);
    }

    public class ResponseRepresentation
    {
        public required KeyPair KeyPair;
        public required string DevicePassword;
        public required DateTime CreatedAt;
        public required string Address;
        public required string DeviceId;
        public required string DeviceUsername;
    }
}

public class ClientCredentials
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}
