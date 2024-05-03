using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Messages;

public class MessagesEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<ListMessagesResponse>> ListMessages(PaginationFilter? pagination = null)
        => await _client.Get<ListMessagesResponse>($"api/{API_VERSION}/Messages", null, pagination);

    public async Task<ApiResponse<ListMessagesResponse>> ListMessages(IEnumerable<string> ids, PaginationFilter? pagination = null)
        => await _client.Request<ListMessagesResponse>(HttpMethod.Get, $"api/{API_VERSION}/Messages")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("ids", ids)
            .Execute();

    public async Task<ApiResponse<Message>> GetMessage(string id, bool noBody = false) => await _client.Request<Message>(HttpMethod.Get, $"api/{API_VERSION}/Messages/{id}")
        .Authenticate()
        .AddQueryParameter("noBody", noBody)
        .Execute();

    public async Task<ApiResponse<SendMessageResponse>> SendMessage(SendMessageRequest request) => await _client.Post<SendMessageResponse>($"api/{API_VERSION}/Messages", request);
}
