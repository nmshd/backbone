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
    private readonly string _nonExistantDeviceId = "DVC00000000000000000";

    public DevicesStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, ISignatureHelper signatureHelper, ChallengesApi challengesApi, IdentitiesApi identitiesApi, DevicesApi devicesApi) :
        base(httpConfiguration, signatureHelper, challengesApi, identitiesApi, devicesApi)
    {
        _keyPair = signatureHelper.CreateKeyPair();
    }

    #region StepDefinitions

    [Given(@"an Identity i with a device d1")]
    public async Task GivenAnIdentityIWithADeviceD1()
    {
        _createChallengeResponse = await CreateChallenge();
        _createChallengeResponse.Should().NotBeNull();

        _createIdentityResponse = await CreateIdentity(_createChallengeResponse.Content.Result, _keyPair);
        _createIdentityResponse.Should().NotBeNull();

        _deviceIdD1 = _createIdentityResponse.Content.Result!.Device.Id;
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

    [When(@"a DELETE request is sent to the Devices/\{id} endpoint with ""?(.*?)""?")]
    public async Task WhenADELETERequestIsSentToTheDeviceIdEndpointWithD2Id(string description)
    {
        var deviceId = GetDeviceId(description);
        _deletionResponse = await DeleteUnOnboardedDevice(deviceId);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_deletionResponse!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    [Then(@"d(.*) is deleted")]
    public async Task ThenDIsDeleted(int index)
    {
        var deviceId = GetDeviceId(index);
        var response = await ListDevices();

        response.Content.Result!.Count.Should().Be(1);
        response.Content.Result!.First().Id?.StringValue.Should().NotBe(deviceId);
    }

    [Then(@"d(.*) is not deleted")]
    public async Task ThenDIsNotDeleted(int index)
    {
        var deviceId = GetDeviceId(index);
        var response = await ListDevices();

        response.Content.Result!.Where(d => d.Id!.StringValue == deviceId).Should().NotBeEmpty();
    }

    #endregion

    private string? GetDeviceId(int index)
    {
        var deviceId = index switch
        {
            1 => _deviceIdD1,
            2 => _deviceIdD2,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
        return deviceId;
    }

    private string? GetDeviceId(string description)
    {
        var deviceId = description switch
        {
            "d1.Id" => _deviceIdD1,
            "d2.Id" => _deviceIdD2,
            "a non existent id" => _nonExistantDeviceId,
            _ => throw new ArgumentOutOfRangeException(nameof(description))
        };
        return deviceId;
    }

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
