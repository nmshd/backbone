using System.Collections.Specialized;
using Backbone.AdminApi.Sdk.Endpoints.Tokens.Response;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Tokens;

public class TokensEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<ListTokensResponse>> ListTokensByIdentity(PaginationFilter paginationFilter, string createdBy, CancellationToken cancellationToken)
    {
        var queryParameters = new NameValueCollection
        {
            { "createdBy", createdBy }
        };
        return await _client.Get<ListTokensResponse>($"api/{API_VERSION}/Tokens", queryParameters, paginationFilter);
    }

    public async Task<ApiResponse<EmptyResponse>> ResetAccessFailedCount(string tokenId, CancellationToken cancellationToken)
    {
        return await _client.Patch<EmptyResponse>($"api/{API_VERSION}/Tokens/{tokenId}/ResetAccessFailedCount");
    }
}
