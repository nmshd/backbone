﻿using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
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
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    public DevicesStepDefinitions(ChallengesContext challengesContext, ResponseContext responseContext, HttpClientFactory factory,
        IOptions<HttpConfiguration> httpConfiguration, ClientPool clientPool)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);

        _challengesContext = challengesContext;
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    #endregion

    #region Given

    [Given($"an Identity {RegexFor.SINGLE_THING} with a Device {RegexFor.SINGLE_THING}")]
    public async Task GivenAnIdentityWithADevice(string identityName, string deviceName)
    {
        var client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);
        _clientPool.Add(client).ForIdentity(identityName).AndDevice(deviceName);
    }

    [Given($"an Identity {RegexFor.SINGLE_THING} with a Device {RegexFor.SINGLE_THING} and an unonboarded Device {RegexFor.SINGLE_THING}")]
    public async Task GivenAnIdentityWithADeviceAndAnUnonboardedDevice(string identityName, string onboardedDeviceName, string unonboardedDeviceName)
    {
        var client = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);
        _clientPool.Add(client).ForIdentity(identityName).AndDevice(onboardedDeviceName);
        var clientForUnOnboardedDevice = await client.OnboardNewDevice("Passw0rd");
        _clientPool.Add(clientForUnOnboardedDevice).ForIdentity(identityName).AndDevice(unonboardedDeviceName);
    }

    [Given($"an Identity {RegexFor.SINGLE_THING} with Devices {RegexFor.LIST_OF_THINGS}")]
    public async Task GivenAnIdentityWithDevices(string identityName, string deviceNamesString)
    {
        var clientOfFirstDevice = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, DEVICE_PASSWORD);

        var deviceNames = SplitNames(deviceNamesString);
        var firstDeviceName = deviceNames.First();

        _clientPool.Add(clientOfFirstDevice).ForIdentity(identityName).AndDevice(firstDeviceName);

        foreach (var deviceName in deviceNames.Skip(1))
        {
            var additionalDevice = await clientOfFirstDevice.OnboardNewDevice("Passw0rd");
            _clientPool.Add(additionalDevice).ForIdentity(identityName).AndDevice(deviceName);
        }
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Devices endpoint with a valid signature on {RegexFor.SINGLE_THING}")]
    public async Task WhenIdentitySendsAPostRequestToTheDevicesEndpointWithASignedChallenge(string identityName, string challengeName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var signedChallenge = CreateSignedChallenge(identity, _challengesContext.Challenges[challengeName]);

        _responseContext.WhenResponse = await identity.Devices.RegisterDevice(new RegisterDeviceRequest
        {
            DevicePassword = DEVICE_PASSWORD,
            SignedChallenge = signedChallenge
        });
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Devices/Self endpoint with the communication language '([a-z]{{2}})'")]
    public async Task WhenDeviceSendsAPutRequestToTheDevicesSelfEndpointWithTheCommunicationLanguage(string deviceName, string communicationLanguage)
    {
        _communicationLanguage = communicationLanguage;
        var request = new UpdateActiveDeviceRequest { CommunicationLanguage = _communicationLanguage };

        var client = _clientPool.GetForDeviceName(deviceName);
        _responseContext.WhenResponse = await client.Devices.UpdateActiveDevice(request);
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Devices/Self endpoint with a non-existent language code")]
    public async Task WhenDeviceSendsAPutRequestToTheDeviceSelfEndpointWithAnInvalidPayload(string deviceName)
    {
        var request = new UpdateActiveDeviceRequest { CommunicationLanguage = "xz" };
        var client = _clientPool.GetForDeviceName(deviceName);
        _responseContext.WhenResponse = await client.Devices.UpdateActiveDevice(request);
    }

    [When($"{RegexFor.SINGLE_THING} sends a PUT request to the /Devices/Self/Password endpoint with the new password '([^']*)'")]
    public async Task WhenDeviceSendsAPutRequestToTheDevicesSelfPasswordEndpointWithTheNewPassword(string deviceName, string newPassword)
    {
        var client = _clientPool.GetForDeviceName(deviceName);

        var oldPassword = client.DeviceData!.UserCredentials.Password;
        var request = new ChangePasswordRequest { OldPassword = oldPassword, NewPassword = newPassword };

        _responseContext.WhenResponse = await client.Devices.ChangePassword(request);
    }

    [When($"{RegexFor.SINGLE_THING} sends a DELETE request to the /Devices/{{id}} endpoint with {RegexFor.SINGLE_THING}.Id")]
    public async Task WhenDeviceSendsADeleteRequestToTheDeviceIdEndpointWithTheDeviceId(string senderDeviceName, string deviceName)
    {
        var deviceId = _clientPool.GetForDeviceName(deviceName).DeviceData!.DeviceId;

        var client = _clientPool.GetForDeviceName(senderDeviceName);

        _responseContext.WhenResponse = await client.Devices.DeleteDevice(deviceId);
    }

    [When($"{RegexFor.SINGLE_THING} sends a DELETE request to the /Devices/{{id}} endpoint with a non existent id")]
    public async Task WhenDeviceSendsADeleteRequestToTheDeviceIdEndpointWithNonExistentId(string deviceName)
    {
        var client = _clientPool.GetForDeviceName(deviceName);
        _responseContext.WhenResponse = await client.Devices.DeleteDevice("DVC00000000000000000");
    }

    #endregion

    #region Then

    [Then($"{RegexFor.SINGLE_THING} is deleted")]
    public async Task ThenDeviceIsDeleted(string deviceName)
    {
        var clientOfDeletedDevice = _clientPool.GetForDeviceName(deviceName);
        var clientOfOtherDevice = _clientPool.FirstForIdentityAddress(clientOfDeletedDevice.IdentityData!.Address);

        var deviceId = clientOfDeletedDevice.DeviceData!.DeviceId;

        var response = await clientOfOtherDevice.Devices.ListDevices();
        response.Result!.Count.Should().Be(1);
        response.Result!.First().Id.Should().NotBe(deviceId);
    }

    [Then($"{RegexFor.SINGLE_THING} is not deleted")]
    public async Task ThenDeviceIsNotDeleted(string deviceName)
    {
        var identityName = _clientPool.GetIdentityNameForDevice(deviceName)!;
        var deviceId = _clientPool.GetForDeviceName(deviceName).DeviceData!.DeviceId;

        var client = _clientPool.FirstForIdentityName(identityName);

        var response = await client.Devices.ListDevices();
        response.Result!.Where(d => d.Id == deviceId).Should().NotBeEmpty();
    }

    [Then($"the Backbone has persisted '([a-z]{{2}})' as the new communication language of {RegexFor.SINGLE_THING}.")]
    public async Task ThenTheBackboneHasPersistedAsTheNewCommunicationLanguageOfDevice(string communicationLanguage, string deviceName)
    {
        var client = _clientPool.GetForDeviceName(deviceName);

        var response = await client.Devices.ListDevices();
        response.Result!.Count.Should().Be(1);
        response.Result!.First().CommunicationLanguage.Should().Be(_communicationLanguage);
    }

    #endregion
}
