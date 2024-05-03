using Backbone.AdminApi.Sdk.Endpoints.Relationships.Types.Responses;
using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Relationships;

public class RelationshipsEndpoint(EndpointClient client) : AdminApiEndpoint(client)
{
    public async Task<ApiResponse<ListRelationshipsResponse>> GetAllRelationships(PaginationFilter? pagination = null)
        => await _client.Get<ListRelationshipsResponse>($"api/{API_VERSION}/Relationships", null, pagination);

    public async Task<ApiResponse<ListRelationshipsResponse>> GetAllRelationships(string participant, PaginationFilter? pagination = null) => await _client
        .Request<ListRelationshipsResponse>(HttpMethod.Get, $"api/{API_VERSION}/Relationships")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("participant", participant)
        .Execute();
}
