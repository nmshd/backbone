using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.Utils;
using static Backbone.ConsumerApi.Tests.Integration.Support.Constants;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "DELETE Device")]
[Scope(Feature = "UPDATE Device")]
[Scope(Feature = "POST Device")]
internal class DevicesStepDefinitions
{
    private Client _sdk = null!;
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;

    private ApiResponse<EmptyResponse>? _deletionResponse;
    private ApiResponse<EmptyResponse>? _updateResponse;

    private string? _communicationLanguage;
    private string? _deviceIdD1;
    private string? _deviceIdD2;
    private const string NON_EXISTENT_DEVICE_ID = "DVC00000000000000000";

    private readonly DevicesContext _devicesContext;
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public DevicesStepDefinitions(DevicesContext devicesContext, IdentitiesContext identitiesContext, ResponseContext responseContext, HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);

        _devicesContext = devicesContext;
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private Challenge Challenge => _responseContext.ChallengeResponse!.Result!;
    private DeviceData Device(string deviceName) => _devicesContext.Devices[deviceName];
    private Client Identity(string identityName) => _identitiesContext.Identities[identityName];

    [Given("an Identity ([a-zA-Z0-9]+) with a device ([a-zA-Z0-9]+)")]
    public async Task GivenAnIdentityIWithADevice(string identityName, string deviceName)
    {
        await CreateAuthenticated(_identitiesContext, _httpClient, _clientCredentials, identityName);
        _devicesContext.Devices.Add(deviceName, Identity(identityName).DeviceData!);
    }

    [Given(@"an Identity ([a-zA-Z0-9]+) with devices? ([a-zA-Z0-9, ]+)")]
    public async Task GivenAnIdentityIWithDevicesDAndD(string identityName, string deviceNames)
    {
        await CreateAuthenticated(_identitiesContext, _httpClient, _clientCredentials, identityName);

        foreach (var deviceName in SplitNames(deviceNames))
            _devicesContext.Devices.Add(deviceName, Identity(identityName).DeviceData!);
    }

    [Given("an un-onboarded device ([a-zA-Z0-9]+) that belongs to ([a-zA-Z0-9]+)")]
    public async Task GivenAnUnOnboardedDeviceDThatBelongsToI(string deviceName, string identityName)
    {
        var client = await Identity(identityName).OnboardNewDevice("deviceTwoPassword");
        _devicesContext.Devices.Add(deviceName, client.DeviceData!);
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Devices endpoint")]
    public async Task WhenISendsAPOSTRequestToTheDevicesEndpoint(string identityName)
    {
        var signedChallenge = CreateSignedChallenge(Identity(identityName), Challenge);

        _responseContext.WhenResponse = _responseContext.RegisterDeviceResponse = await Identity(identityName).Devices.RegisterDevice(new RegisterDeviceRequest
        {
            DevicePassword = DEVICE_PASSWORD,
            SignedChallenge = signedChallenge
        });
    }

    [When(@"([a-zA-Z0-9]+) sends a PUT request to the Devices/Self endpoint with the communication language '(.*)'")]
    public async Task WhenISendsAPutRequestToTheDevicesSelfEndpointWithTheCommunicationLanguage(string identityName, string communicationLanguage)
    {
        _communicationLanguage = communicationLanguage;
        var request = new UpdateActiveDeviceRequest { CommunicationLanguage = _communicationLanguage };

        _responseContext.WhenResponse = _responseContext.UpdateDeviceResponse = await Identity(identityName).Devices.UpdateActiveDevice(request);
    }

    [When("([a-zA-Z0-9]+) sends a PUT request to the Devices/Self endpoint with a non-existent language code")]
    public async Task WhenISendsAPutRequestToTheDeviceSelfEndpointWithAnInvalidPayload(string identityName)
    {
        var request = new UpdateActiveDeviceRequest { CommunicationLanguage = "xz" };
        _responseContext.WhenResponse = _responseContext.UpdateDeviceResponse = await Identity(identityName).Devices.UpdateActiveDevice(request);
    }

    [When(@"([a-zA-Z0-9]+) sends a PUT request to the Devices/Self/Password endpoint with the new password '([^']*)'")]
    public async Task WhenISendsAPutRequestToTheDevicesSelfPasswordEndpointWithTheNewPassword(string identityName, string newPassword)
    {
        var oldPassword = Identity(identityName).DeviceData!.UserCredentials.Password;
        var request = new ChangePasswordRequest { OldPassword = oldPassword, NewPassword = newPassword };

        _responseContext.WhenResponse = await Identity(identityName).Devices.ChangePassword(request);
    }

    [When("([a-zA-Z0-9]+) sends a DELETE request to the Devices/{id} endpoint with ([a-zA-Z0-9]+).Id")]
    public async Task WhenISendsADeleteRequestToTheDeviceIdEndpointWithTheDeviceId(string identityName, string deviceName)
    {
        _responseContext.WhenResponse = _responseContext.DeleteDeviceResponse = await Identity(identityName).Devices.DeleteDevice(Device(deviceName).DeviceId);
    }

    [When("([a-zA-Z0-9]+) sends a DELETE request to the Devices/{id} endpoint with a non existent id")]
    public async Task WhenISendsADeleteRequestToTheDeviceIdEndpointWithNonExistentId(string identityName)
    {
        _responseContext.WhenResponse = _responseContext.DeleteDeviceResponse = await Identity(identityName).Devices.DeleteDevice(NON_EXISTENT_DEVICE_ID);
    }

    [Then("the device on the Backbone of ([a-zA-Z0-9]+) has the new communication language")]
    public async Task ThenTheDeviceOnTheBackboneOfIHasTheNewCommunicationLanguage(string identityName)
    {
        var response = await ListDevices(identityName);
        response.Result!.Count.Should().Be(1);
        response.Result!.First().CommunicationLanguage.Should().Be(_communicationLanguage);
    }

    protected async Task<ApiResponse<ListDevicesResponse>> ListDevices(string identityName = "i")
    {
        var response = await Identity(identityName).Devices.ListDevices();
        response.Should().BeASuccess();

        return response;
    }

    [Then("([a-zA-Z0-9]+) of ([a-zA-Z0-9]+) is deleted")]
    public async Task ThenDOfIIsDeleted(string deviceName, string identityName)
    {
        var response = await ListDevices(identityName);
        response.Result!.Count.Should().Be(1);
        response.Result!.First().Id.Should().NotBe(Device(deviceName).DeviceId);
    }

    [Then("([a-zA-Z0-9]+) of ([a-zA-Z0-9]+) is not deleted")]
    public async Task ThenDIsNotDeleted(string deviceName, string identityName)
    {
        var response = await ListDevices(identityName);
        response.Result!.Where(d => d.Id == Device(deviceName).DeviceId).Should().NotBeEmpty();
    }
}

public class DevicesContext
{
    public readonly Dictionary<string, DeviceData> Devices = new();
}
