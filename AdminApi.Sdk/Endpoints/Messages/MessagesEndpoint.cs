using Backbone.AdminApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Messages;

public class MessagesEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<ListMessagesResponse>> GetMessagesWithParticipant(string identityAddress, string type)
    {
        return await _client.Get<ListMessagesResponse>($"api/{API_VERSION}/Messages?&participant={identityAddress}&type={type}");
    }
}
