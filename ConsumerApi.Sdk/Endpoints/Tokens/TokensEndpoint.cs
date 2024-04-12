﻿using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens;

public class TokensEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<CreateTokenResponse>> CreateToken(CreateTokenRequest request)
        => await _client.Post<CreateTokenResponse>("Tokens", request);

    public async Task<ApiResponse<ListTokensResponse>> ListTokens(PaginationFilter? pagination = null)
        => await _client.Get<ListTokensResponse>("Tokens", null, pagination);

    public async Task<ApiResponse<ListTokensResponse>> ListTokens(IEnumerable<string> ids, PaginationFilter? pagination = null) => await _client
        .Request<ListTokensResponse>(HttpMethod.Get, "Tokens")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("ids", ids)
        .Execute();

    public async Task<ApiResponse<Token>> GetToken(string id) => await _client.GetUnauthenticated<Token>($"Tokens/{id}");
}
