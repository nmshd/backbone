using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
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

    public DevicesStepDefinitions(ChallengesContext challengesContext, IdentitiesContext identitiesContext, ResponseContext responseContext, HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
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
    public async Task GivenAnIdentityIWithADevice(string identityName, string deviceName)
    {
        var client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);
        ClientPool.Add(client).ForIdentity(identityName).AndDevice(deviceName);
    }

    [Given(@"an Identity ([a-zA-Z0-9]+) with devices ([a-zA-Z0-9, ]+)")]
    public async Task GivenAnIdentityIWithDevices(string identityName, string deviceNames)
    {
        var client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);

        foreach (var deviceName in SplitNames(deviceNames))
            ClientPool.Add(client).ForIdentity(identityName).AndDevice(deviceName);
    }

    [Given("an un-onboarded device ([a-zA-Z0-9]+) that belongs to ([a-zA-Z0-9]+)")]
    public async Task GivenAnUnOnboardedDeviceDThatBelongsToI(string deviceName, string identityName)
    {
        var client = await ClientPool.FirstForIdentity(identityName)!.OnboardNewDevice("deviceTwoPassword");
        ClientPool.Add(client).ForIdentity(identityName).AndDevice(deviceName);
    }
    #endregion

    #region When
    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Devices endpoint with a valid signature on ([a-zA-Z0-9]+)")]
    public async Task WhenISendsAPostRequestToTheDevicesEndpointWithASignedChallenge(string identityName, string challengeName)
    {
        var identity = ClientPool.FirstForIdentity(identityName)!;
        var signedChallenge = CreateSignedChallenge(identity, _challengesContext.Challenges[challengeName]);

        _responseContext.WhenResponse = _responseContext.RegisterDeviceResponse = await identity.Devices.RegisterDevice(new RegisterDeviceRequest
        {
            DevicePassword = DEVICE_PASSWORD,
            SignedChallenge = signedChallenge
        });
    }

    [When(@"([a-zA-Z0-9]+) sends a PUT request to the /Devices/Self endpoint with the communication language '(de|en|it|pt)'")]
    public async Task WhenDSendsAPutRequestToTheDevicesSelfEndpointWithTheCommunicationLanguage(string deviceName, string communicationLanguage)
    {
        _communicationLanguage = communicationLanguage;
        var request = new UpdateActiveDeviceRequest { CommunicationLanguage = _communicationLanguage };

        _responseContext.WhenResponse = _responseContext.UpdateDeviceResponse = await ClientPool.GetForDevice(deviceName)!.Devices.UpdateActiveDevice(request);
    }

    [When("([a-zA-Z0-9]+) sends a PUT request to the /Devices/Self endpoint with a non-existent language code")]
    public async Task WhenDSendsAPutRequestToTheDeviceSelfEndpointWithAnInvalidPayload(string deviceName)
    {
        var request = new UpdateActiveDeviceRequest { CommunicationLanguage = "xz" };
        _responseContext.WhenResponse = _responseContext.UpdateDeviceResponse = await ClientPool.GetForDevice(deviceName)!.Devices.UpdateActiveDevice(request);
    }

    [When(@"([a-zA-Z0-9]+) sends a PUT request to the /Devices/Self/Password endpoint with the new password '([^']*)'")]
    public async Task WhenDSendsAPutRequestToTheDevicesSelfPasswordEndpointWithTheNewPassword(string deviceName, string newPassword)
    {
        var oldPassword = ClientPool.GetForDevice(deviceName)!.DeviceData!.UserCredentials.Password;
        var request = new ChangePasswordRequest { OldPassword = oldPassword, NewPassword = newPassword };

        _responseContext.WhenResponse = await ClientPool.GetForDevice(deviceName)!.Devices.ChangePassword(request);
    }

    [When("([a-zA-Z0-9]+) sends a DELETE request to the /Devices/{id} endpoint with ([a-zA-Z0-9]+).Id")]
    public async Task WhenDSendsADeleteRequestToTheDeviceIdEndpointWithTheDeviceId(string senderDeviceName, string deviceName)
    {
        var deviceId = ClientPool.GetForDevice(deviceName)!.DeviceData!.DeviceId;
        _responseContext.WhenResponse = _responseContext.DeleteDeviceResponse = await ClientPool.GetForDevice(senderDeviceName)!.Devices.DeleteDevice(deviceId);
    }

    [When("([a-zA-Z0-9]+) sends a DELETE request to the /Devices/{id} endpoint with a non existent id")]
    public async Task WhenDSendsADeleteRequestToTheDeviceIdEndpointWithNonExistentId(string deviceName)
    {
        _responseContext.WhenResponse = _responseContext.DeleteDeviceResponse = await ClientPool.GetForDevice(deviceName)!.Devices.DeleteDevice(NON_EXISTENT_DEVICE_ID);
    }
    #endregion

    #region Then
    [Then("([a-zA-Z0-9]+) is deleted")]
    public async Task ThenDIsDeleted(string deviceName)
    {
        var identityName = ClientPool.GetIdentityForDevice(deviceName)!;
        var deviceId = ClientPool.GetForDevice(deviceName)!.DeviceData!.DeviceId;

        var response = await ListDevices(identityName);
        response.Result!.Count.Should().Be(1);
        response.Result!.First().Id.Should().NotBe(deviceId);
    }

    [Then("([a-zA-Z0-9]+) is not deleted")]
    public async Task ThenDIsNotDeleted(string deviceName)
    {
        var identityName = ClientPool.GetIdentityForDevice(deviceName)!;
        var deviceId = ClientPool.GetForDevice(deviceName)!.DeviceData!.DeviceId;

        var response = await ListDevices(identityName);
        response.Result!.Where(d => d.Id == deviceId).Should().NotBeEmpty();
    }

    [Then(@"the Backbone has persisted '(de|en|pt)' as the new communication language of ([a-zA-Z0-9]+)\.")]
    public async Task ThenTheBackboneHasPersistedAsTheNewCommunicationLanguageOfD(string communicationLanguage, string deviceName)
    {
        var identityName = ClientPool.GetIdentityForDevice(deviceName)!;

        var response = await ListDevices(identityName);
        response.Result!.Count.Should().Be(1);
        response.Result!.First().CommunicationLanguage.Should().Be(_communicationLanguage);
    }
    #endregion

    protected async Task<ApiResponse<ListDevicesResponse>> ListDevices(string identityName = "i")
    {
        var response = await ClientPool.FirstForIdentity(identityName)!.Devices.ListDevices();
        response.Should().BeASuccess();

        return response;
    }
}
