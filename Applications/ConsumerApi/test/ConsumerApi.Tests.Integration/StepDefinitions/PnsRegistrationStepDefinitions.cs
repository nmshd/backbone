using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Requests;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class PnsRegistrationStepDefinitions
{
    private readonly IdentitiesContext _identitiesContext;
    private readonly ResponseContext _responseContext;

    public PnsRegistrationStepDefinitions(IdentitiesContext identitiesContext, ResponseContext responseContext)
    {
        _identitiesContext = identitiesContext;
        _responseContext = responseContext;
    }

    private Client Identity(string identityName) => _identitiesContext.Identities[identityName];

    [When("([a-zA-Z0-9]+) sends a PUT request to the /Devices/Self/PushNotifications endpoint")]
    public async Task WhenAPutRequestIsSentToTheDevicesSelfPushNotificationsEndpoint(string identityName)
    {
        var request = new UpdateDeviceRegistrationRequest
        {
            Platform = "dummy",
            Handle = "eXYs0v3XT9w:APA91bHal6RzkPdjiFmoXvtVRJlfN81OCyzVIbXx4bTQupfcUQmDY9eAdUABLntZzO4M5rv7jmcj3Mk6",
            AppId = "someAppId"
        };

        _responseContext.WhenResponse = _responseContext.UpdateDeviceRegistrationResponse = await Identity(identityName).PushNotifications.RegisterForPushNotifications(request);
    }

    [When(@"([a-zA-Z0-9]+) sends a DELETE request to the /Devices/Self/PushNotifications endpoint")]
    public async Task WhenADeleteRequestIsSentToTheDevicesSelfPushNotificationsEndpoint(string identityName)
    {
        _responseContext.WhenResponse = await Identity(identityName).PushNotifications.UnregisterFromPushNotifications();
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Devices/Self/PushNotifications/SendTestNotification endpoint")]
    public async Task WhenISendsAPostRequestToTheDevicesSelfPushNotificationsSendTestNotificationEndpoint(string identityName)
    {
        _responseContext.WhenResponse = await Identity(identityName).PushNotifications.SendTestNotification(new object());
    }
}
