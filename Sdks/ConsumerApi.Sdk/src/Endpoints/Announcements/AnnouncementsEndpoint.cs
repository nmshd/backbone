using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Announcements.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Announcements;

public class AnnouncementsEndpoint : ConsumerApiEndpoint
{
    public AnnouncementsEndpoint(EndpointClient client) : base(client)
    {
    }

    public async Task<ApiResponse<ListAnnouncementsResponse>> ListAnnouncements(string language, PaginationFilter? pagination = null)
    {
        return await _client.Request<ListAnnouncementsResponse>(HttpMethod.Get, $"api/{API_VERSION}/Announcements")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("language", language)
            .Execute();
    }
}
