using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications;

public class PushNotificationsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<UpdateDeviceRegistrationResponse>> RegisterForPushNotifications(UpdateDeviceRegistrationRequest request)
        => await _client.Put<UpdateDeviceRegistrationResponse>("Devices/Self/PushNotifications", request);

    public async Task<ConsumerApiResponse<EmptyResponse>> UnregisterFromPushNotifications()
        => await _client.Delete<EmptyResponse>("Devices/Self/PushNotifications");

    public async Task<ConsumerApiResponse<EmptyResponse>> SendTestNotification(object data)
        => await _client.Post<EmptyResponse>("Devices/Self/PushNotifications/SendTestNotification", data);
}
