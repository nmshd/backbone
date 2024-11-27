using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates;

public class RelationshipTemplatesEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<ListRelationshipTemplatesResponse>> ListTemplates(IEnumerable<ListRelationshipTemplatesQueryItem> queryItems, PaginationFilter? pagination = null)
    {
        var request = _client
            .Request<ListRelationshipTemplatesResponse>(HttpMethod.Get, $"api/{API_VERSION}/RelationshipTemplates")
            .Authenticate()
            .WithPagination(pagination);

        var i = 0;
        foreach (var queryItem in queryItems)
        {
            request.AddQueryParameter($"templates.{i}.id", queryItem.Id);

            if (queryItem.Password != null)
                request.AddQueryParameter($"templates.{i}.password", queryItem.Password);

            i++;
        }

        return await request.Execute();
    }

    public async Task<ApiResponse<RelationshipTemplate>> GetTemplate(string id)
    {
        return await _client.Get<RelationshipTemplate>($"api/{API_VERSION}/RelationshipTemplates/{id}");
    }

    public async Task<ApiResponse<RelationshipTemplate>> GetTemplate(string id, byte[] password)
    {
        return await _client.Get<RelationshipTemplate>($"api/{API_VERSION}/RelationshipTemplates/{id}?password={Convert.ToBase64String(password)}");
    }

    public async Task<ApiResponse<CreateRelationshipTemplateResponse>> CreateTemplate(CreateRelationshipTemplateRequest request)
    {
        return await _client.Post<CreateRelationshipTemplateResponse>($"api/{API_VERSION}/RelationshipTemplates", request);
    }

    public async Task<ApiResponse<EmptyResponse>> DeleteTemplate(string id)
    {
        return await _client.Delete<EmptyResponse>($"api/{API_VERSION}/RelationshipTemplates/{id}");
    }
}
