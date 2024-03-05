using Backbone.AdminApi.Sdk.Endpoints.Common;
using Backbone.AdminApi.Sdk.Endpoints.Common.Types;
using Backbone.AdminApi.Sdk.Endpoints.Relationships.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Relationships;

public class RelationshipsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<AdminApiResponse<List<Relationship>>> GetAllRelationships(string participant, PaginationFilter? pagination = null)
        => await _client.Get<List<Relationship>>("Relationships", null, pagination);
}
