using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tags.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Tags;

public class TagsEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<ListTagsResponse>> ListTags()
    {
        return await _client.GetUnauthenticated<ListTagsResponse>($"api/{API_VERSION}/Tags");
    }
}
