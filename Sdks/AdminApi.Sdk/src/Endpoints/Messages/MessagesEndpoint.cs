using Backbone.AdminApi.Sdk.Endpoints.Messages.Types;
using Backbone.AdminApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Messages;

public class MessagesEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<ListMessagesResponse>> ListMessages(string participant, MessageType type)
    {
        return await _client
            .Request<ListMessagesResponse>(HttpMethod.Get, $"api/{API_VERSION}/Messages")
            .Authenticate()
            .AddQueryParameter("participant", participant)
            .AddQueryParameter("type", type.Value)
            .Execute();
    }
}
