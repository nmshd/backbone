using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Crypto;
using Backbone.Crypto.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Backbone.ConsumerApi.Tests.Integration.Helpers.ThrowHelpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "DELETE Device")]
internal class DevicesStepDefinitions : BaseStepDefinitions
{
    private readonly KeyPair _keyPair1;
    private readonly KeyPair _keyPair2;

    private HttpResponse<CreateIdentityResponse>? _createIdentityResponse1;
    private HttpResponse<CreateIdentityResponse>? _createIdentityResponse2;
    private HttpResponse? _deletionResponse;

    private string? _deviceIdD1;
    private string? _deviceIdD2;
    private string? _deviceIdD3;
    private const string NON_EXISTENT_DEVICE_ID = "DVC00000000000000000";

    public DevicesStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, ISignatureHelper signatureHelper, ChallengesApi challengesApi, IdentitiesApi identitiesApi, DevicesApi devicesApi) :
        base(httpConfiguration, signatureHelper, challengesApi, identitiesApi, devicesApi)
    {
        _keyPair1 = signatureHelper.CreateKeyPair();
        _keyPair2 = signatureHelper.CreateKeyPair();
    }

    #region StepDefinitions

    [Given("an Identity i1? with a device d1?")]
    public async Task GivenAnIdentityIWithADeviceD()
    {
        _createIdentityResponse1 = await CreateIdentity(keyPair: _keyPair1);
        _createIdentityResponse1.Should().NotBeNull();

        _deviceIdD1 = _createIdentityResponse1.Content.Result!.Device.Id;
    }

    [Given("an Identity i2 with a device d2")]
    public async Task GivenAnIdentityI2WithADeviceD2()
    {
        _createIdentityResponse2 = await CreateIdentity(keyPair: _keyPair2);
        _createIdentityResponse2.Should().NotBeNull();

        _deviceIdD2 = _createIdentityResponse2.Content.Result!.Device.Id;
    }

    [Given("the current user uses d1?")]
    public void GivenTheCurrentUserUsesD1()
    {
        var username = _createIdentityResponse1!.Content.Result!.Device.Username;
        Authenticate(username, "test");
    }

    [Given("an un-onboarded device d2")]
    public async Task GivenAnUnOnboardedDeviceD2()
    {
        var deviceResponse = await RegisterDevice(_keyPair1);
        _deviceIdD2 = deviceResponse.Content.Result!.Id;
    }


    [Given("an un-onboarded device d3 belonging to i2")]
    public async Task GivenAnUnOnboardedDeviceD3BelongingToI2()
    {
        var usernameD2 = _createIdentityResponse2!.Content.Result!.Device.Username;
        var deviceResponse = await RegisterDevice(_keyPair2, usernameD2);
        _deviceIdD3 = deviceResponse.Content.Result!.Id;
    }

    [When("a DELETE request is sent to the Devices/{id} endpoint with d.Id")]
    public async Task WhenADELETERequestIsSentToTheDeviceIdEndpointWithDId()
    {
        ThrowIfNull(_deviceIdD1);
        _deletionResponse = await DeleteOnboardedDevice(_deviceIdD1);
    }

    [When("a DELETE request is sent to the Devices/{id} endpoint with d2.Id")]
    public async Task WhenADELETERequestIsSentToTheDeviceIdEndpointWithD2Id()
    {
        ThrowIfNull(_deviceIdD2);
        _deletionResponse = await DeleteUnOnboardedDevice(_deviceIdD2);
    }

    [When("a DELETE request is sent to the Devices/{id} endpoint with d3.Id")]
    public async Task WhenADELETERequestIsSentToTheDeviceIdEndpointWithD3Id()
    {
        ThrowIfNull(_createIdentityResponse1);
        ThrowIfNull(_deviceIdD3);
        Authenticate(_createIdentityResponse1.Content.Result!.Device.Username, "test");
        _deletionResponse = await DeleteUnOnboardedDevice(_deviceIdD3);
    }

    [When("a DELETE request is sent to the Devices/{id} endpoint with a non existent id")]
    public async Task WhenADELETERequestIsSentToTheDeviceIdEndpointWithNonExistantId()
    {
        _deletionResponse = await DeleteNonExistentDevice(NON_EXISTENT_DEVICE_ID);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_deletionResponse!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    [Then(@"the error code is ""([^""]*)""")]
    public void ThenTheErrorCodeIs(string errorCode)
    {
        ThrowIfNull(_deletionResponse);
        var actualErrorCode = _deletionResponse.Content!.Error?.Code;
        actualErrorCode.Should().Be(errorCode);
    }

    [Then("d is not deleted")]
    public async Task ThenDIsNotDeleted()
    {
        var response = await ListDevices();
        response.Content.Result!.Where(d => d.Id == _deviceIdD1).Should().NotBeEmpty();
    }

    [Then("d1 is not deleted")]
    public async Task ThenD1IsNotDeleted()
    {
        var response = await ListDevices();
        response.Content.Result!.Where(d => d.Id == _deviceIdD1).Should().NotBeEmpty();
    }

    [Then("d2 is deleted")]
    public async Task ThenD2IsDeleted()
    {
        var response = await ListDevices();
        response.Content.Result!.Count.Should().Be(1);
        response.Content.Result!.First().Id.Should().NotBe(_deviceIdD2);
    }

    [Then("d3 is not deleted")]
    public async Task ThenD3IsNotDeleted()
    {
        var response = await ListDevices();
        response.Content.Result!.Should().Contain(d => d.Id == _deviceIdD3);
    }

    #endregion

    private async Task<HttpResponse<RegisterDeviceResponse>> RegisterDevice(KeyPair keyPair, string? username = null)
    {
        var challenge = (await CreateChallenge()).Content.Result!;

        var serializedChallenge = JsonConvert.SerializeObject(challenge);

        var signature = _signatureHelper.CreateSignature(keyPair.PrivateKey, ConvertibleString.FromUtf8(serializedChallenge));

        dynamic signedChallenge = new
        {
            sig = signature.BytesRepresentation,
            alg = 2
        };

        var registerDeviceRequest = new RegisterDeviceRequest()
        {
            DevicePassword = "test",

            SignedChallenge = new RegisterDeviceRequestSignedChallenge()
            {
                Challenge = serializedChallenge,
                Signature = (ConvertibleString.FromUtf8(JsonConvert.SerializeObject(signedChallenge)) as ConvertibleString)!.Base64Representation
            }
        };

        var requestConfiguration = _requestConfiguration.Clone();
        requestConfiguration.ContentType = "application/json";

        if (username != null)
            requestConfiguration.AuthenticationParameters.Username = username;

        requestConfiguration.SetContent(registerDeviceRequest);

        var deviceResponse = await _devicesApi.RegisterDevice(requestConfiguration);
        deviceResponse.IsSuccessStatusCode.Should().BeTrue();

        return deviceResponse;
    }

    protected async Task<HttpResponse> DeleteUnOnboardedDevice(string id)
    {
        var response = await _devicesApi.DeleteDevice(_requestConfiguration, id);
        response.IsSuccessStatusCode.Should().BeTrue();

        return response;
    }

    protected async Task<HttpResponse> DeleteOnboardedDevice(string id)
    {
        var response = await _devicesApi.DeleteDevice(_requestConfiguration, id);
        response.IsSuccessStatusCode.Should().BeFalse();

        return response;
    }

    protected async Task<HttpResponse> DeleteNonExistentDevice(string id)
    {
        var response = await _devicesApi.DeleteDevice(_requestConfiguration, id);
        response.IsSuccessStatusCode.Should().BeFalse();

        return response;
    }

    protected async Task<HttpResponse<ListDevicesResponse>> ListDevices()
    {
        var response = await _devicesApi.ListDevices(_requestConfiguration);
        response.IsSuccessStatusCode.Should().BeTrue();

        return response;
    }
}
