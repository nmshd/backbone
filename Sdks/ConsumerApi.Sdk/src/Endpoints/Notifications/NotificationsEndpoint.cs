using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Notifications.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Notifications;

public class NotificationsEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<EmptyResponse>> SendNotification(SendNotificationRequest request)
    {
        return await _client.Post<EmptyResponse>($"api/{API_VERSION}/Notifications", request);
    }
}
