using System.Net;
using Backbone.ConsumerApi.Tests.Integration.API;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.ConsumerApi.Tests.Integration.Models;
using Backbone.Crypto.Abstractions;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RequestConfiguration = Backbone.ConsumerApi.Tests.Integration.Models.RequestConfiguration;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "PUT /Devices/Self/PushNotifications")]
internal class PnsRegistrationStepDefinitions : BaseStepDefinitions
{
    // keep in mind: these tests use DummyPushService so they will not execute the implemented code

    private readonly PushNotificationsApi _pnsRegistrationsApi;
    private HttpResponse<UpdateDeviceRegistrationResponse>? _response;

    public PnsRegistrationStepDefinitions(
        IOptions<HttpConfiguration> httpConfiguration, ISignatureHelper signatureHelper, ChallengesApi challengesApi, IdentitiesApi identitiesApi, DevicesApi devicesApi, PushNotificationsApi pnsRegistrationsApi) :
        base(httpConfiguration, signatureHelper, challengesApi, identitiesApi, devicesApi)
    {
        _pnsRegistrationsApi = pnsRegistrationsApi;
    }

    [When(@"a PUT request is sent to the /Devices/Self/PushNotifications endpoint")]
    public async Task WhenAPUTRequestIsSentToTheDevicesSelfPushNotificationsEndpoint()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(_requestConfiguration);
        requestConfiguration.AuthenticationParameters.Username = "USRa";
        requestConfiguration.AuthenticationParameters.Password = "a";

        requestConfiguration.ContentType = "application/json";

        requestConfiguration.Content = JsonConvert.SerializeObject(new PnsRegistrationRequest()
        {
            Platform = "fcm",
            Handle = "eXYs0v3XT9w:APA91bHal6RzkPdjiFmoXvtVRJlfN81OCyzVIbXx4bTQupfcUQmDY9eAdUABLntZzO4M5rv7jmcj3Mk6",
            AppId = "someAppId"
        });

        _response = await _pnsRegistrationsApi.RegisterForPushNotifications(requestConfiguration);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int statusCode)
    {
        ThrowHelpers.ThrowIfNull(_response);
        _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

    [Then(@"the response contains the push identifier for the device")]
    public void ThenTheResponseContainsThePushIdentifierForTheDevice()
    {
        _response!.Content.Result!.DevicePushIdentifier.Should().NotBeNullOrEmpty();
    }
}
