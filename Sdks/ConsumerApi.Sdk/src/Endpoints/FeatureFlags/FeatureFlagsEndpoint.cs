using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.FeatureFlags.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.FeatureFlags.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.FeatureFlags;

public class FeatureFlagsEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<EmptyResponse>> ChangeFeatureFlags(ChangeFeatureFlagsRequest request)
    {
        return await _client.Patch<EmptyResponse>($"api/{API_VERSION}/Identities/Self/FeatureFlags", request);
    }

    public async Task<ApiResponse<ListFeatureFlagsResponse>> ListFeatureFlags(string identityAddress)
    {
        return await _client.Get<ListFeatureFlagsResponse>($"api/{API_VERSION}/Identities/{identityAddress}/FeatureFlags");
    }
}
