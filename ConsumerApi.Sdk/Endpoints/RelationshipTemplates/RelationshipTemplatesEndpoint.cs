using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates;

public class RelationshipTemplatesEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<List<RelationshipTemplate>>> ListTemplates(PaginationFilter? pagination = null) => await _client.Get<List<RelationshipTemplate>>("RelationshipTemplates", null, pagination);

    public async Task<ConsumerApiResponse<List<RelationshipTemplate>>> ListTemplates(List<string> ids, PaginationFilter? pagination = null) => await _client
        .Request<List<RelationshipTemplate>>(HttpMethod.Get, "RelationshipTemplates")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("ids", ids)
        .Execute();

    public async Task<ConsumerApiResponse<RelationshipTemplate>> GetTemplate(string id) => await _client.Get<RelationshipTemplate>($"RelationshipTemplates/{id}");

    public async Task<ConsumerApiResponse<CreateRelationshipTemplateResponse>> CreateTemplate(CreateRelationshipTemplateRequest request) => await _client.Post<CreateRelationshipTemplateResponse>("RelationshipTemplates", request);
}
