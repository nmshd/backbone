using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates;

public class RelationshipTemplatesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<ListRelationshipTemplatesResponse>> ListTemplates(PaginationFilter? pagination = null)
        => await _client.Get<ListRelationshipTemplatesResponse>("RelationshipTemplates", null, pagination);

    public async Task<ApiResponse<ListRelationshipTemplatesResponse>> ListTemplates(IEnumerable<string> ids, PaginationFilter? pagination = null)
        => await _client
        .Request<ListRelationshipTemplatesResponse>(HttpMethod.Get, "RelationshipTemplates")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("ids", ids)
        .Execute();

    public async Task<ApiResponse<RelationshipTemplate>> GetTemplate(string id) => await _client.Get<RelationshipTemplate>($"RelationshipTemplates/{id}");

    public async Task<ApiResponse<CreateRelationshipTemplateResponse>> CreateTemplate(CreateRelationshipTemplateRequest request)
        => await _client.Post<CreateRelationshipTemplateResponse>("RelationshipTemplates", request);
}
