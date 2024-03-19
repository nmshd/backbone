using Backbone.AdminApi.Sdk.Endpoints.Common;
using Backbone.AdminApi.Sdk.Endpoints.Common.Types;
using Backbone.AdminApi.Sdk.Endpoints.Relationships.Types.Responses;

namespace Backbone.AdminApi.Sdk.Endpoints.Relationships;

public class RelationshipsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<AdminApiResponse<ListRelationshipsResponse>> GetAllRelationships(PaginationFilter? pagination = null)
        => await _client.Get<ListRelationshipsResponse>("Relationships", null, pagination);

    public async Task<AdminApiResponse<ListRelationshipsResponse>> GetAllRelationships(string participant, PaginationFilter? pagination = null) => await _client
        .Request<ListRelationshipsResponse>(HttpMethod.Get, "Relationships")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("participant", participant)
        .Execute();
}
