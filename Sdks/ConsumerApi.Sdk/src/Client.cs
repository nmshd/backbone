using System.Text.Json;
using Backbone.BuildingBlocks.SDK.Crypto;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Announcements;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Docs;
using Backbone.ConsumerApi.Sdk.Endpoints.FeatureFlags;
using Backbone.ConsumerApi.Sdk.Endpoints.Files;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities;
using Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages;
using Backbone.ConsumerApi.Sdk.Endpoints.Notifications;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications;
using Backbone.ConsumerApi.Sdk.Endpoints.Quotas;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tags;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;

namespace Backbone.ConsumerApi.Sdk;

public class Client
{
    private readonly Configuration _configuration;
    private readonly HttpClient _httpClient;

    private Client(HttpClient httpClient, Configuration configuration, DeviceData? deviceData, IdentityData? identityData)
    {
        IAuthenticator authenticator = deviceData != null ? new OAuthAuthenticator(configuration.Authentication, httpClient) : new AnonymousAuthenticator();
        var endpointClient = new EndpointClient(httpClient, authenticator, configuration.JsonSerializerOptions);

        _configuration = configuration;
        _httpClient = httpClient;
        DeviceData = deviceData;
        IdentityData = identityData;

        Announcements = new AnnouncementsEndpoint(endpointClient);
        Challenges = new ChallengesEndpoint(endpointClient);
        Datawallet = new DatawalletEndpoint(endpointClient);
        Devices = new DevicesEndpoint(endpointClient);
        Docs = new DocsEndpoint(httpClient);
        Files = new FilesEndpoint(endpointClient);
        FeatureFlags = new FeatureFlagsEndpoint(endpointClient);
        Identities = new IdentitiesEndpoint(endpointClient);
        Messages = new MessagesEndpoint(endpointClient);
        Notifications = new NotificationsEndpoint(endpointClient);
        PushNotifications = new PushNotificationsEndpoint(endpointClient);
        Quotas = new QuotasEndpoint(endpointClient);
        Relationships = new RelationshipsEndpoint(endpointClient);
        RelationshipTemplates = new RelationshipTemplatesEndpoint(endpointClient);
        SyncRuns = new SyncRunsEndpoint(endpointClient);
        Tags = new TagsEndpoint(endpointClient);
        Tokens = new TokensEndpoint(endpointClient);
    }

    public DeviceData? DeviceData { get; }
    public IdentityData? IdentityData { get; }

    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public AnnouncementsEndpoint Announcements { get; }
    public ChallengesEndpoint Challenges { get; }
    public DatawalletEndpoint Datawallet { get; }
    public DevicesEndpoint Devices { get; }
    public FeatureFlagsEndpoint FeatureFlags { get; }
    public FilesEndpoint Files { get; }
    public IdentitiesEndpoint Identities { get; }
    public MessagesEndpoint Messages { get; }
    public NotificationsEndpoint Notifications { get; }
    public PushNotificationsEndpoint PushNotifications { get; }
    public QuotasEndpoint Quotas { get; }
    public RelationshipsEndpoint Relationships { get; }
    public RelationshipTemplatesEndpoint RelationshipTemplates { get; }
    public SyncRunsEndpoint SyncRuns { get; }
    public TagsEndpoint Tags { get; }
    public TokensEndpoint Tokens { get; }
    public DocsEndpoint Docs { get; set; }

    // ReSharper restore UnusedAutoPropertyAccessor.Global

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
                ClientCredentials = clientCredentials,
                UserCredentials = null
            }
        };

        return new Client(httpClient, configuration, null, null);
    }

    public static Client CreateForExistingIdentity(string baseUrl, ClientCredentials clientCredentials, UserCredentials userCredentials)
    {
        return CreateForExistingIdentity(new HttpClient { BaseAddress = new Uri(baseUrl) }, clientCredentials, userCredentials);
    }

    public static Client CreateForExistingIdentity(HttpClient httpClient, ClientCredentials clientCredentials, UserCredentials userCredentials)
    {
        if (httpClient.BaseAddress == null)
            throw new Exception("The base address of the HttpClient must be set.");

        var configuration = new Configuration
        {
            Authentication = new Configuration.AuthenticationConfiguration
            {
                ClientCredentials = clientCredentials,
                UserCredentials = userCredentials
            }
        };

        var client = new Client(httpClient, configuration, new DeviceData { DeviceId = "", UserCredentials = userCredentials }, null);

        return client;
    }

    public static async Task<Client> CreateForNewIdentity(string baseUrl, ClientCredentials clientCredentials, string password)
    {
        return await CreateForNewIdentity(new HttpClient { BaseAddress = new Uri(baseUrl) }, clientCredentials, password);
    }

    public static async Task<Client> CreateForNewIdentity(HttpClient httpClient, ClientCredentials clientCredentials, string password)
    {
        if (httpClient.BaseAddress == null)
            throw new Exception("The base address of the HttpClient must be set.");

        var (identityData, deviceData) = await CreateIdentity(httpClient, clientCredentials, password);

        var configuration = new Configuration
        {
            Authentication = new Configuration.AuthenticationConfiguration
            {
                ClientCredentials = clientCredentials,
                UserCredentials = deviceData.UserCredentials
            }
        };

        var client = new Client(httpClient, configuration, deviceData, identityData);

        // Run a datawallet version upgrade sync run to create a datawallet for the identity
        var syncRun = await client.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = SyncRunType.DatawalletVersionUpgrade, Duration = 30 }, 1);
        await client.SyncRuns.FinalizeDatawalletVersionUpgrade(syncRun.Result!.SyncRun.Id, new FinalizeDatawalletVersionUpgradeRequest { DatawalletModifications = [], NewDatawalletVersion = 1 });

        return client;
    }

    private static async Task<(IdentityData identityData, DeviceData deviceData)> CreateIdentity(HttpClient httpClient, ClientCredentials clientCredentials, string password)
    {
        var temporaryConfiguration = new Configuration
        {
            Authentication = new Configuration.AuthenticationConfiguration
            {
                ClientCredentials = clientCredentials,
                UserCredentials = null
            }
        };

        var client = new Client(httpClient, temporaryConfiguration, null, null);

        var (keyPair, signedChallenge) = await CreateSignedChallenge(client);

        var createIdentityPayload = new CreateIdentityRequest
        {
            ClientId = client._configuration.Authentication.ClientCredentials.ClientId,
            ClientSecret = client._configuration.Authentication.ClientCredentials.ClientSecret,
            IdentityVersion = 1,
            SignedChallenge = signedChallenge,
            IdentityPublicKey = ConvertibleString.FromUtf8(JsonSerializer.Serialize(new CryptoSignaturePublicKey
            {
                alg = CryptoExchangeAlgorithm.ECDH_X25519,
                pub = keyPair.PublicKey.Base64Representation
            })).BytesRepresentation,
            CommunicationLanguage = "en",
            DevicePassword = password
        };

        var createIdentityResponse = await client.Identities.CreateIdentity(createIdentityPayload);

        if (createIdentityResponse.IsError)
            throw new Exception($"There was an error when creating the identity. The error code was '{createIdentityResponse.Error.Code}'. The message was '{createIdentityResponse.Error.Message}'.");

        var deviceData = new DeviceData
        {
            DeviceId = createIdentityResponse.Result.Device.Id,
            UserCredentials = new UserCredentials(createIdentityResponse.Result.Device.Username, password)
        };

        var identityData = new IdentityData
        {
            Address = createIdentityResponse.Result.Address,
            KeyPair = keyPair
        };

        return (identityData, deviceData);
    }

    public async Task<Client> OnboardNewDevice(string password)
    {
        return await OnboardNewDevice(password, false);
    }

    public async Task<Client> OnboardNewBackupDevice(string password)
    {
        return await OnboardNewDevice(password, true);
    }

    private async Task<Client> OnboardNewDevice(string password, bool isBackupDevice)
    {
        if (DeviceData == null)
            throw new Exception("The device data is missing. This is probably because you're using an unauthenticated client. In order to onboard a new device, the client needs to be authenticated.");

        var (_, signedChallenge) = await CreateSignedChallenge(this);

        var createDeviceResponse = await Devices.RegisterDevice(new RegisterDeviceRequest
        {
            DevicePassword = password,
            SignedChallenge = signedChallenge,
            IsBackupDevice = isBackupDevice
        });

        if (createDeviceResponse.IsError)
            throw new Exception($"There was an error when creating the device. The error code was '{createDeviceResponse.Error.Code}'. The message was '{createDeviceResponse.Error.Message}'.");

        var configuration = new Configuration
        {
            Authentication = new Configuration.AuthenticationConfiguration
            {
                ClientCredentials = _configuration.Authentication.ClientCredentials,
                UserCredentials = new UserCredentials(createDeviceResponse.Result.Username, password)
            }
        };

        var newDeviceData = new DeviceData
        {
            DeviceId = createDeviceResponse.Result.Id,
            UserCredentials = new UserCredentials(createDeviceResponse.Result.Username, password)
        };

        var client = new Client(_httpClient, configuration, newDeviceData, IdentityData);

        return client;
    }

    private static async Task<(KeyPair keyPair, SignedChallenge signedChallenge)> CreateSignedChallenge(Client client)
    {
        var signatureHelper = SignatureHelper.CreateEd25519WithRawKeyFormat();

        var keyPair = client.IdentityData?.KeyPair ?? signatureHelper.CreateKeyPair();

        var createChallengeResponse = await client.Challenges.CreateChallengeUnauthenticated();
        if (createChallengeResponse.IsError)
            throw new Exception(
                $"There was an error when creating a challenge for the new identity. The error code was '{createChallengeResponse.Error.Code}'. The message was '{createChallengeResponse.Error.Message}'.");

        var serializedChallenge = JsonSerializer.Serialize(createChallengeResponse.Result);

        var challengeSignature = signatureHelper.CreateSignature(keyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));
        var signedChallenge = new SignedChallenge(serializedChallenge, challengeSignature);
        return (keyPair, signedChallenge);
    }
}
