using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Microsoft.Extensions.Options;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.Utils;
using static Backbone.ConsumerApi.Tests.Integration.Support.Constants;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class DevicesStepDefinitions
{
    #region Constructor, Fields, Properties

    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;

    private string? _communicationLanguage;

    private readonly ChallengesContext _challengesContext;
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public DevicesStepDefinitions(ChallengesContext challengesContext, IdentitiesContext identitiesContext, ResponseContext responseContext, HttpClientFactory factory,
        IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);

        _challengesContext = challengesContext;
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private ClientPool ClientPool => _identitiesContext.ClientPool;

    #endregion

    #region Given

    [Given("an Identity ([a-zA-Z0-9]+) with a device ([a-zA-Z0-9]+)")]
    public async Task GivenAnIdentityWithADevice(string identityName, string deviceName)
    {
        var client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);
        ClientPool.Add(client).ForIdentity(identityName).AndDevice(deviceName);
    }

    [Given(@"an Identity ([a-zA-Z0-9]+) with devices ([a-zA-Z0-9, ]+)")]
    public async Task GivenAnIdentityWithDevices(string identityName, string deviceNamesString)
    {
        var clientOfFirstDevice = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);

        var deviceNames = SplitNames(deviceNamesString);
        var firstDeviceName = deviceNames.First();

        ClientPool.Add(clientOfFirstDevice).ForIdentity(identityName).AndDevice(firstDeviceName);

        foreach (var deviceName in deviceNames.Skip(1))
        {
            var additionalDevice = await clientOfFirstDevice.OnboardNewDevice("Passw0rd");
            ClientPool.Add(additionalDevice).ForIdentity(identityName).AndDevice(deviceName);
        }
    }

    [Given("an un-onboarded device ([a-zA-Z0-9]+) that belongs to ([a-zA-Z0-9]+)")]
    public async Task GivenAnUnOnboardedDeviceThatBelongsToIdentity(string deviceName, string identityName)
    {
        var existingClient = ClientPool.FirstForIdentityName(identityName);
        var clientForUnOnboardedDevice = await existingClient.OnboardNewDevice("Passw0rd");
        ClientPool.Add(clientForUnOnboardedDevice).ForIdentity(identityName).AndDevice(deviceName);
    }

    #endregion

    #region When

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Devices endpoint with a valid signature on ([a-zA-Z0-9]+)")]
    public async Task WhenIdentitySendsAPostRequestToTheDevicesEndpointWithASignedChallenge(string identityName, string challengeName)
    {
        var identity = ClientPool.FirstForIdentityName(identityName)!;
        var signedChallenge = CreateSignedChallenge(identity, _challengesContext.Challenges[challengeName]);

        _responseContext.WhenResponse = _responseContext.RegisterDeviceResponse = await identity.Devices.RegisterDevice(new RegisterDeviceRequest
        {
            DevicePassword = DEVICE_PASSWORD,
            SignedChallenge = signedChallenge
        });
    }

    [When(@"([a-zA-Z0-9]+) sends a PUT request to the /Devices/Self endpoint with the communication language '(de|en|it|pt)'")]
    public async Task WhenDeviceSendsAPutRequestToTheDevicesSelfEndpointWithTheCommunicationLanguage(string deviceName, string communicationLanguage)
    {
        _communicationLanguage = communicationLanguage;
        var request = new UpdateActiveDeviceRequest { CommunicationLanguage = _communicationLanguage };

        var client = ClientPool.GetForDeviceName(deviceName);
        _responseContext.WhenResponse = _responseContext.UpdateDeviceResponse = await client!.Devices.UpdateActiveDevice(request);
    }

    [When("([a-zA-Z0-9]+) sends a PUT request to the /Devices/Self endpoint with a non-existent language code")]
    public async Task WhenDeviceSendsAPutRequestToTheDeviceSelfEndpointWithAnInvalidPayload(string deviceName)
    {
        var request = new UpdateActiveDeviceRequest { CommunicationLanguage = "xz" };
        var client = ClientPool.GetForDeviceName(deviceName);
        _responseContext.WhenResponse = _responseContext.UpdateDeviceResponse = await client!.Devices.UpdateActiveDevice(request);
    }

    [When(@"([a-zA-Z0-9]+) sends a PUT request to the /Devices/Self/Password endpoint with the new password '([^']*)'")]
    public async Task WhenDeviceSendsAPutRequestToTheDevicesSelfPasswordEndpointWithTheNewPassword(string deviceName, string newPassword)
    {
        var client = ClientPool.GetForDeviceName(deviceName);

        var oldPassword = client!.DeviceData!.UserCredentials.Password;
        var request = new ChangePasswordRequest { OldPassword = oldPassword, NewPassword = newPassword };

        _responseContext.WhenResponse = await client!.Devices.ChangePassword(request);
    }

    [When("([a-zA-Z0-9]+) sends a DELETE request to the /Devices/{id} endpoint with ([a-zA-Z0-9]+).Id")]
    public async Task WhenDeviceSendsADeleteRequestToTheDeviceIdEndpointWithTheDeviceId(string senderDeviceName, string deviceName)
    {
        var deviceId = ClientPool.GetForDeviceName(deviceName)!.DeviceData!.DeviceId;

        var client = ClientPool.GetForDeviceName(senderDeviceName);

        _responseContext.WhenResponse = _responseContext.DeleteDeviceResponse = await client!.Devices.DeleteDevice(deviceId);
    }

    [When("([a-zA-Z0-9]+) sends a DELETE request to the /Devices/{id} endpoint with a non existent id")]
    public async Task WhenDeviceSendsADeleteRequestToTheDeviceIdEndpointWithNonExistentId(string deviceName)
    {
        var client = ClientPool.GetForDeviceName(deviceName);
        _responseContext.WhenResponse = _responseContext.DeleteDeviceResponse = await client!.Devices.DeleteDevice(NON_EXISTENT_DEVICE_ID);
    }

    #endregion

    #region Then

    [Then("([a-zA-Z0-9]+) is deleted")]
    public async Task ThenDeviceIsDeleted(string deviceName)
    {
        var deviceId = ClientPool.GetForDeviceName(deviceName)!.DeviceData!.DeviceId;

        var clientOfDeletedDevice = ClientPool.GetForDeviceName(deviceName);
        var clientOfOtherDevice = ClientPool.FirstForIdentityAddress(clientOfDeletedDevice.IdentityData!.Address);

        var response = await clientOfOtherDevice.Devices.ListDevices();
        response.Result!.Count.Should().Be(1);
        response.Result!.First().Id.Should().NotBe(deviceId);
    }

    [Then("([a-zA-Z0-9]+) is not deleted")]
    public async Task ThenDeviceIsNotDeleted(string deviceName)
    {
        var identityName = ClientPool.GetIdentityForDevice(deviceName)!;
        var deviceId = ClientPool.GetForDeviceName(deviceName)!.DeviceData!.DeviceId;

        var client = ClientPool.FirstForIdentityName(identityName);

        var response = await client!.Devices.ListDevices();
        response.Result!.Where(d => d.Id == deviceId).Should().NotBeEmpty();
    }

    [Then(@"the Backbone has persisted '(de|en|pt)' as the new communication language of ([a-zA-Z0-9]+)\.")]
    public async Task ThenTheBackboneHasPersistedAsTheNewCommunicationLanguageOfDevice(string communicationLanguage, string deviceName)
    {
        var client = ClientPool.GetForDeviceName(deviceName);

        var response = await client!.Devices.ListDevices();
        response.Result!.Count.Should().Be(1);
        response.Result!.First().CommunicationLanguage.Should().Be(_communicationLanguage);
    }

    #endregion
}
