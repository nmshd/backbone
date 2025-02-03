using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Tokens.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Tokens;

public class TokensEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<CreateTokenResponse>> CreateToken(CreateTokenRequest request)
    {
        return await _client.Post<CreateTokenResponse>($"api/{API_VERSION}/Tokens", request);
    }

    public async Task<ApiResponse<EmptyResponse>> CreateTokenUnauthenticated(CreateTokenRequest request)
    {
        return await _client.PostUnauthenticated<EmptyResponse>($"api/{API_VERSION}/Tokens", request);
    }

    public async Task<ApiResponse<ListTokensResponse>> ListTokens(PaginationFilter? pagination = null)
    {
        return await _client.Get<ListTokensResponse>($"api/{API_VERSION}/Tokens", null, pagination);
    }

    public async Task<ApiResponse<ListTokensResponse>> ListTokens(IEnumerable<string> queryItems, PaginationFilter? pagination = null)
    {
        var request = _client
            .Request<ListTokensResponse>(HttpMethod.Get, $"api/{API_VERSION}/Tokens")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("ids", queryItems);

        return await request.Execute();
    }

    public async Task<ApiResponse<Token>> GetTokenUnauthenticated(string id)
    {
        return await _client.GetUnauthenticated<Token>($"api/{API_VERSION}/Tokens/{id}");
    }

    public async Task<ApiResponse<Token>> GetTokenUnauthenticated(string id, byte[] password)
    {
        return await _client.GetUnauthenticated<Token>($"api/{API_VERSION}/Tokens/{id}?password={Convert.ToBase64String(password)}");
    }

    public async Task<ApiResponse<Token>> GetToken(string id)
    {
        return await _client.Get<Token>($"api/{API_VERSION}/Tokens/{id}");
    }

    public async Task<ApiResponse<Token>> GetToken(string id, byte[] password)
    {
        return await _client.Get<Token>($"api/{API_VERSION}/Tokens/{id}?password={Convert.ToBase64String(password)}");
    }

    public async Task<ApiResponse<EmptyResponse>> DeleteToken(string id)
    {
        return await _client.Delete<EmptyResponse>($"api/{API_VERSION}/Tokens/{id}");
    }
}
