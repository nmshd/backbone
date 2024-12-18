using Backbone.AdminApi.Sdk.Endpoints.Announcements.Types;
using Backbone.AdminApi.Sdk.Endpoints.Announcements.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Announcements.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Announcements;

public class AnnouncementsEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<Announcement>> CreateAnnouncement(CreateAnnouncementRequest request)
    {
        return await _client.Post<Announcement>($"api/{API_VERSION}/Announcements", request);
    }

    public async Task<ApiResponse<GetAllAnnouncementsResponse>> GetAllAnnouncements()
    {
        return await _client.Get<GetAllAnnouncementsResponse>($"api/{API_VERSION}/Announcements");
    }
}
