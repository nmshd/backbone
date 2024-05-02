using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.PushNotifications;

public class PushNotificationsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<UpdateDeviceRegistrationResponse>> RegisterForPushNotifications(UpdateDeviceRegistrationRequest request)
        => await _client.Put<UpdateDeviceRegistrationResponse>($"api/{API_VERSION}/Devices/Self/PushNotifications", request);

    public async Task<ApiResponse<EmptyResponse>> UnregisterFromPushNotifications()
        => await _client.Delete<EmptyResponse>($"api/{API_VERSION}/Devices/Self/PushNotifications");

    public async Task<ApiResponse<EmptyResponse>> SendTestNotification(object data)
        => await _client.Post<EmptyResponse>($"api/{API_VERSION}/Devices/Self/PushNotifications/SendTestNotification", data);
}
