using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Tags.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Tags;

public class TagsEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<CachedApiResponse<ListTagsResponse>> ListTags(CacheControl? cacheControl = null)
    {
        return await _client.GetCachedUnauthenticated<ListTagsResponse>($"api/{API_VERSION}/Tags", null, null, cacheControl);
    }
}
