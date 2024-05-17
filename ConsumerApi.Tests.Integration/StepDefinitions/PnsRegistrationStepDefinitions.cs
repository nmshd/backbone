using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Responses;
using Backbone.ConsumerApi.Tests.Integration.Configuration;
using Backbone.Tooling;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "PUT /Devices/Self/PushNotifications")]
internal class PnsRegistrationStepDefinitions
{
    private Client? _sdk;
    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _httpClient;
    private ApiResponse<UpdateDeviceRegistrationResponse>? _response;

    public PnsRegistrationStepDefinitions(HttpClientFactory factory, IOptions<HttpConfiguration> httpConfiguration)
    {
        _httpClient = factory.CreateClient();
        _clientCredentials = new ClientCredentials(httpConfiguration.Value.ClientCredentials.ClientId, httpConfiguration.Value.ClientCredentials.ClientSecret);
    }

    [Given("the user is authenticated")]
    public async Task GivenTheUserIsAuthenticated()
    {
        _sdk = await Client.CreateForNewIdentity(_httpClient, _clientCredentials, PasswordHelper.GeneratePassword(20, 26));
    }

    [When("a PUT request is sent to the /Devices/Self/PushNotifications endpoint")]
    public async Task WhenAPutRequestIsSentToTheDevicesSelfPushNotificationsEndpoint()
    {
        var request = new UpdateDeviceRegistrationRequest
        {
            Platform = "fcm",
            Handle = "eXYs0v3XT9w:APA91bHal6RzkPdjiFmoXvtVRJlfN81OCyzVIbXx4bTQupfcUQmDY9eAdUABLntZzO4M5rv7jmcj3Mk6",
            AppId = "someAppId"
        };

        _response = await _sdk!.PushNotifications.RegisterForPushNotifications(request);
    }

    [Then(@"the response status code is (\d\d\d) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        ((int)_response!.Status).Should().Be(expectedStatusCode);
    }

    [Then("the response contains the push identifier for the device")]
    public void ThenTheResponseContainsThePushIdentifierForTheDevice()
    {
        _response!.Result!.DevicePushIdentifier.Should().NotBeNullOrEmpty();
    }
}
