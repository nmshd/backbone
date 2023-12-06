using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Crypto;
using Backbone.Crypto.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "DELETE Device")]
internal class DevicesStepDefinitions : BaseStepDefinitions
{
    private readonly KeyPair? _keyPair;

    private HttpResponse<Challenge>? _createChallengeResponse;
    private HttpResponse<CreateIdentityResponse>? _createIdentityResponse;
    private HttpResponse? _deletionResponse;

    private string? _deviceIdD1;
    private string? _deviceIdD2;
    private const string NON_EXISTENT_DEVICE_ID = "DVC00000000000000000";

    public DevicesStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, ISignatureHelper signatureHelper, ChallengesApi challengesApi, IdentitiesApi identitiesApi, DevicesApi devicesApi) :
        base(httpConfiguration, signatureHelper, challengesApi, identitiesApi, devicesApi)
    {
        _keyPair = signatureHelper.CreateKeyPair();
    }

    #region StepDefinitions

    [Given(@"an Identity i with a device d")]
    public async Task GivenAnIdentityIWithADeviceD()
    {
        _createChallengeResponse = await CreateChallenge();
        _createChallengeResponse.Should().NotBeNull();

        _createIdentityResponse = await CreateIdentity(_createChallengeResponse.Content.Result, _keyPair);
        _createIdentityResponse.Should().NotBeNull();

        _deviceIdD1 = _createIdentityResponse.Content.Result!.Device.Id;
    }

    [Given(@"an Identity i with a device d1")]
    public async Task GivenAnIdentityIWithADeviceD1()
    {
        _createChallengeResponse = await CreateChallenge();
        _createChallengeResponse.Should().NotBeNull();

        _createIdentityResponse = await CreateIdentity(_createChallengeResponse.Content.Result, _keyPair);
        _createIdentityResponse.Should().NotBeNull();

        _deviceIdD1 = _createIdentityResponse.Content.Result!.Device.Id;
    }



    [Given(@"the current user uses d")]
    public void GivenTheCurrentUserUsesD()
    {
        var username = _createIdentityResponse!.Content.Result!.Device.Username;
        Authenticate(username, "test");
    }

    [Given(@"the current user uses d1")]
    public void GivenTheCurrentUserUsesD1()
    {
        var username = _createIdentityResponse!.Content.Result!.Device.Username;
        Authenticate(username, "test");
    }



    [Given(@"an un-onboarded device d2")]
    public async Task GivenAnUnOnboardedDeviceD2()
    {
        var deviceResponse = await RegisterDevice(_createChallengeResponse!.Content.Result, _keyPair);
        _deviceIdD2 = deviceResponse.Content.Result!.Id;
    }



    [When(@"a DELETE request is sent to the Devices/{id} endpoint with d.Id")]
    public async Task WhenADELETERequestIsSentToTheDeviceIdEndpointWithDId()
    {
        _deletionResponse = await DeleteUnOnboardedDevice(_deviceIdD1);
    }

    [When(@"a DELETE request is sent to the Devices/{id} endpoint with d2.Id")]
    public async Task WhenADELETERequestIsSentToTheDeviceIdEndpointWithD2Id()
    {
        _deletionResponse = await DeleteUnOnboardedDevice(_deviceIdD2);
    }

    [When(@"a DELETE request is sent to the Devices/{id} endpoint with a non existent id")]
    public async Task WhenADELETERequestIsSentToTheDeviceIdEndpointWithNonExistantId()
    {
        _deletionResponse = await DeleteUnOnboardedDevice(NON_EXISTENT_DEVICE_ID);
    }



    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_deletionResponse!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }




    [Then(@"d is not deleted")]
    public async Task ThenDIsNotDeleted()
    {
        var response = await ListDevices();
        response.Content.Result!.Where(d => d.Id!.StringValue == _deviceIdD1).Should().NotBeEmpty();
    }

    [Then(@"d1 is not deleted")]
    public async Task ThenD1IsNotDeleted()
    {
        var response = await ListDevices();
        response.Content.Result!.Where(d => d.Id!.StringValue == _deviceIdD1).Should().NotBeEmpty();
    }

    [Then(@"d2 is deleted")]
    public async Task ThenD2IsDeleted()
    {
        var response = await ListDevices();
        response.Content.Result!.Count.Should().Be(1);
        response.Content.Result!.First().Id?.StringValue.Should().NotBe(_deviceIdD2);
    }

    #endregion

    protected async Task<HttpResponse<RegisterDeviceResponse>> RegisterDevice(Challenge? challenge = null, KeyPair? keyPair = null)
    {
        challenge ??= (await CreateChallenge()).Content.Result!;
        keyPair ??= _signatureHelper.CreateKeyPair();

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
        requestConfiguration.SetContent(registerDeviceRequest);

        var deviceResponse = await _devicesApi.RegisterDevice(requestConfiguration);
        deviceResponse.IsSuccessStatusCode.Should().BeTrue();

        return deviceResponse;
    }

    protected async Task<HttpResponse> DeleteUnOnboardedDevice(string? id)
    {
        var response = await _devicesApi.DeleteDevice($"/Devices/{id}", _requestConfiguration, id);
        //response.IsSuccessStatusCode.Should().BeTrue();

        return response;
    }

    protected async Task<HttpResponse<ListDevicesResponse>> ListDevices()
    {
        var response = await _devicesApi.ListDevices(_requestConfiguration);
        response.IsSuccessStatusCode.Should().BeTrue();

        return response;
    }
}
