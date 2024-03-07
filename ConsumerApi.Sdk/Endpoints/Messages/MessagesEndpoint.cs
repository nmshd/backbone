using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Messages;

public class MessagesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<ListMessagesResponse>> ListMessages(PaginationFilter? pagination = null)
        => await _client.Get<ListMessagesResponse>("Messages", null, pagination);
    public async Task<ConsumerApiResponse<ListMessagesResponse>> ListMessages(IEnumerable<string> ids, PaginationFilter? pagination = null)
        => await _client.Request<ListMessagesResponse>(HttpMethod.Get, "Messages")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("ids", ids)
            .Execute();

    public async Task<ConsumerApiResponse<Message>> GetMessage(string id, bool noBody = false) => await _client.Request<Message>(HttpMethod.Get, $"Messages/{id}")
        .Authenticate()
        .AddQueryParameter("noBody", noBody)
        .Execute();

    public async Task<ConsumerApiResponse<SendMessageResponse>> SendMessage(SendMessageRequest request) => await _client.Post<SendMessageResponse>("Messages", request);
}
