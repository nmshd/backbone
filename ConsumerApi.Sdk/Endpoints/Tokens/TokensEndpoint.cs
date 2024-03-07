using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens;

public class TokensEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<CreateTokenResponse>> CreateToken(CreateTokenRequest request) => await _client.Post<CreateTokenResponse>("Tokens", request);

    public async Task<ConsumerApiResponse<ListTokensResponse>> ListTokens(PaginationFilter? pagination = null)
        => await _client.Get<ListTokensResponse>("Tokens", null, pagination);

    public async Task<ConsumerApiResponse<ListTokensResponse>> ListTokens(IEnumerable<string> ids, PaginationFilter? pagination = null) => await _client
        .Request<ListTokensResponse>(HttpMethod.Get, "Tokens")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("ids", ids)
        .Execute();

    public async Task<ConsumerApiResponse<Token>> GetToken(string id) => await _client.GetUnauthenticated<Token>($"Tokens/{id}");
}
