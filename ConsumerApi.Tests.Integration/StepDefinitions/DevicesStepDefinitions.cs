using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Extensions;
using Backbone.ConsumerApi.Tests.Integration.Support;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "DELETE Device")]
[Scope(Feature = "UPDATE Device")]
internal class DevicesStepDefinitions
{
    private Client _sdk = null!;
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;

    private ApiResponse<EmptyResponse>? _deletionResponse;
    private ApiResponse<EmptyResponse>? _updateResponse;

    private string? _deviceIdD1;
    private string? _deviceIdD2;
    private const string NON_EXISTENT_DEVICE_ID = "DVC00000000000000000";

    public DevicesStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
    }

    [Given("an Identity i with a device d1?")]
    public async Task GivenAnIdentityIWithADevice()
    {
        _sdk = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, Constants.DEVICE_PASSWORD);
        _deviceIdD1 = _sdk.DeviceData!.DeviceId;
    }

    [Given("an un-onboarded device d2")]
    public async Task GivenAnUnOnboardedDeviceD2()
    {
        var client = await _sdk.OnboardNewDevice("deviceTwoPassword");
        _deviceIdD2 = client.DeviceData!.DeviceId;
    }

    [When("a DELETE request is sent to the Devices/{id} endpoint with d1?.Id")]
    public async Task WhenADeleteRequestIsSentToTheDeviceIdEndpointWithTheDeviceId()
    {
        _deletionResponse = await _sdk.Devices.DeleteDevice(_deviceIdD1!);
    }

    [When("a DELETE request is sent to the Devices/{id} endpoint with d2.Id")]
    public async Task WhenADeleteRequestIsSentToTheDeviceIdEndpointWithD2Id()
    {
        _deletionResponse = await _sdk.Devices.DeleteDevice(_deviceIdD2!);
    }

    [When("a DELETE request is sent to the Devices/{id} endpoint with a non existent id")]
    public async Task WhenADeleteRequestIsSentToTheDeviceIdEndpointWithNonExistentId()
    {
        _deletionResponse = await _sdk.Devices.DeleteDevice(NON_EXISTENT_DEVICE_ID);
    }

    [When("a PUT request is sent to the Devices/Self endpoint with a valid payload")]
    public async Task WhenAPutRequestIsSentToTheDeviceSelfEndpointWithAValidPayload()
    {
        var request = new UpdateDeviceRequest { CommunicationLanguage = "en" };
        _updateResponse = await _sdk.Devices.UpdateDevice(request);
    }

    [When("a PUT request is sent to the Devices/Self endpoint with an invalid payload")]
    public async Task WhenAPutRequestIsSentToTheDeviceSelfEndpointWithAnInvalidPayload()
    {
        var request = new UpdateDeviceRequest { CommunicationLanguage = "some-non-existent-language" };
        _updateResponse = await _sdk.Devices.UpdateDevice(request);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_deletionResponse != null)
            ((int)_deletionResponse!.Status).Should().Be(expectedStatusCode);

        if (_updateResponse != null)
            ((int)_updateResponse!.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response content contains an error with the error code ""([^""]*)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _deletionResponse!.Error.Should().NotBeNull();
        _deletionResponse.Error!.Code.Should().Be(errorCode);
    }

    [Then("d is not deleted")]
    public async Task ThenDIsNotDeleted()
    {
        var response = await ListDevices();
        response.Result!.Where(d => d.Id == _deviceIdD1).Should().NotBeEmpty();
    }

    [Then("d1 is not deleted")]
    public async Task ThenD1IsNotDeleted()
    {
        var response = await ListDevices();
        response.Result!.Where(d => d.Id == _deviceIdD1).Should().NotBeEmpty();
    }

    [Then("d2 is deleted")]
    public async Task ThenD2IsDeleted()
    {
        var response = await ListDevices();
        response.Result!.Count.Should().Be(1);
        response.Result!.First().Id.Should().NotBe(_deviceIdD2);
    }

    protected async Task<ApiResponse<ListDevicesResponse>> ListDevices()
    {
        var response = await _sdk.Devices.ListDevices();
        response.Should().BeASuccess();

        return response;
    }
}
