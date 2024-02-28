using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Messages;

public class MessagesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<List<Message>>> ListMessages(PaginationFilter? pagination = null)
        => await _client.Get<List<Message>>("Messages", null, pagination);
    public async Task<ConsumerApiResponse<List<Message>>> ListMessages(List<string> ids, PaginationFilter? pagination = null)
        => await _client.Request<List<Message>>(HttpMethod.Get, "Messages")
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
