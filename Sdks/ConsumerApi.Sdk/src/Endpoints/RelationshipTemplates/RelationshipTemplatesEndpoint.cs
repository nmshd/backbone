using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Newtonsoft.Json;

namespace Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates;

public class RelationshipTemplatesEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<ListRelationshipTemplatesResponse>> ListTemplates(IEnumerable<ListRelationshipTemplatesQueryItem> queryItems, PaginationFilter? pagination = null)
    {
        return await _client
            .Request<ListRelationshipTemplatesResponse>(HttpMethod.Get, $"api/{API_VERSION}/RelationshipTemplates")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("templates", queryItems.ToJson())
            .Execute();
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
}

file static class RelationshipTemplateQueryExtensions
{
    public static string ToJson(this IEnumerable<ListRelationshipTemplatesQueryItem> queryItems)
    {
        return JsonConvert.SerializeObject(queryItems, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None });
    }
}
