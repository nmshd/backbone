using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class PnsRegistrationStepDefinitions
{
    private readonly ResponseContext _responseContext;
    private readonly ClientPool _clientPool;

    public PnsRegistrationStepDefinitions(ResponseContext responseContext, ClientPool clientPool)
    {
        _responseContext = responseContext;
        _clientPool = clientPool;
    }

    [When("([a-zA-Z0-9]+) sends a PUT request to the /Devices/Self/PushNotifications endpoint")]
    public async Task WhenIdentitySendsAPutRequestToTheDevicesSelfPushNotificationsEndpoint(string identityName)
    {
        var request = new UpdateDeviceRegistrationRequest
        {
            Platform = "dummy",
            Handle = "eXYs0v3XT9w:APA91bHal6RzkPdjiFmoXvtVRJlfN81OCyzVIbXx4bTQupfcUQmDY9eAdUABLntZzO4M5rv7jmcj3Mk6",
            AppId = "someAppId"
        };

        _responseContext.WhenResponse = _responseContext.UpdateDeviceRegistrationResponse =
            await _clientPool.FirstForIdentityName(identityName).PushNotifications.RegisterForPushNotifications(request);
    }

    [When(@"([a-zA-Z0-9]+) sends a DELETE request to the /Devices/Self/PushNotifications endpoint")]
    public async Task WhenIdentitySendsADeleteRequestToTheDevicesSelfPushNotificationsEndpoint(string identityName)
    {
        _responseContext.WhenResponse = await _clientPool.FirstForIdentityName(identityName).PushNotifications.UnregisterFromPushNotifications();
    }

    [When(@"([a-zA-Z0-9]+) sends a POST request to the /Devices/Self/PushNotifications/SendTestNotification endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheDevicesSelfPushNotificationsSendTestNotificationEndpoint(string identityName)
    {
        _responseContext.WhenResponse = await _clientPool.FirstForIdentityName(identityName).PushNotifications.SendTestNotification(new object());
    }
}
