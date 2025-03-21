using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.FeatureFlags.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.FeatureFlags;

public class FeatureFlagsEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<EmptyResponse>> UpdateFeatureFlags(UpdateFeatureFlagsRequest request)
    {
        return await _client.Patch<EmptyResponse>($"api/{API_VERSION}/Identities/Self/FeatureFlags", request);
    }

    public async Task<ApiResponse<GetFeatureFlagsResponse>> GetFeatureFlags(string identityAddress)
    {
        return await _client.Get<GetFeatureFlagsResponse>($"api/{API_VERSION}/Identities/{identityAddress}/FeatureFlags");
    }
}
