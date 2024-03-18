using Backbone.AdminApi.Sdk.Endpoints.Common;
using Backbone.AdminApi.Sdk.Endpoints.Common.Types;
using Backbone.AdminApi.Sdk.Endpoints.Relationships.Types.Responses;

namespace Backbone.AdminApi.Sdk.Endpoints.Relationships;

public class RelationshipsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<AdminApiResponse<ListRelationshipsResponse>> GetAllRelationships(string participant, PaginationFilter? pagination = null)
        => await _client.Get<ListRelationshipsResponse>("Relationships", null, pagination);
}
