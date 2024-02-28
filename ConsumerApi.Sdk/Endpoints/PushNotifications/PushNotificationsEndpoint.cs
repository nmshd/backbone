using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications;

public class PushNotificationsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<UpdateDeviceRegistrationResponse>> RegisterForPushNotifications(UpdateDeviceRegistrationRequest request)
        => await _client.Put<UpdateDeviceRegistrationResponse>("Devices/Self/PushNotifications", request);

    public async Task<EmptyConsumerApiResponse> UnregisterFromPushNotifications()
        => (EmptyConsumerApiResponse) await _client.Delete<EmptyResponse>("Devices/Self/PushNotifications");

    public async Task<EmptyConsumerApiResponse> SendTestNotification(object data)
        => (EmptyConsumerApiResponse) await _client.Post<EmptyResponse>("Devices/Self/PushNotifications/SendTestNotification", data);
}
