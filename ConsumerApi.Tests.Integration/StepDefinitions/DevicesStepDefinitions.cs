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

    private HttpResponse<Challenge>? _challengeResponse;
    private HttpResponse<CreateIdentityResponse>? _identityResponse;
    private HttpResponse? _response;

    private string? _deviceIdD1;
    private string? _deviceIdD2;

    public DevicesStepDefinitions(IOptions<HttpConfiguration> httpConfiguration, ISignatureHelper signatureHelper, ChallengesApi challengesApi, IdentitiesApi identitiesApi, DevicesApi devicesApi) :
        base(httpConfiguration, signatureHelper, challengesApi, identitiesApi, devicesApi)
    {
        _keyPair = signatureHelper.CreateKeyPair();
    }

    #region StepDefinitions

    [Given(@"an Identity i with a device d1")]
    public async Task GivenAnIdentityIWithADeviceD1()
    {
        _challengeResponse = await CreateChallenge();
        _challengeResponse.Should().NotBeNull();

        _identityResponse = await CreateIdentity(_challengeResponse.Content.Result, _keyPair);
        _identityResponse.Should().NotBeNull();

        _deviceIdD1 = _identityResponse.Content.Result!.Device.Id;
    }

    [Given(@"the current user uses d1")]
    public void GivenTheCurrentUserUsesD1()
    {
        var username = _identityResponse!.Content.Result!.Device.Username;
        Authenticate(username, "test");
    }

    [Given(@"an un-onboarded device d2")]
    public async Task GivenAnUnOnboardedDeviceD2()
    {
        var deviceResponse = await RegisterDevice(_challengeResponse!.Content.Result, _keyPair);
        _deviceIdD2 = deviceResponse.Content.Result!.Id;
    }

    [When(@"a DELETE request is sent to the Devices/\{id} endpoint with ""?(.*?)""?")]
    public async Task WhenADELETERequestIsSentToTheDeviceIdEndpointWithD2Id(string id)
    {
        _response = await DeleteUnOnboardedDevice(_deviceIdD2);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        var actualStatusCode = (int)_response!.StatusCode;
        actualStatusCode.Should().Be(expectedStatusCode);
    }

    [Then(@"d(.*) is deleted")]
    public async Task ThenDIsDeleted(int index)
    {
        var response = await ListDevices();

        response.Content.Result!.Count.Should().Be(1);
        response.Content.Result!.First().Id?.StringValue.Should().Be(_deviceIdD1);
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
        response.IsSuccessStatusCode.Should().BeTrue();

        return response;
    }

    protected async Task<HttpResponse<ListDevicesResponse>> ListDevices()
    {
        var response = await _devicesApi.ListDevices(_requestConfiguration);
        response.IsSuccessStatusCode.Should().BeTrue();

        return response;
    }
}
