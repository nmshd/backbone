﻿using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates;

public class RelationshipTemplatesEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<ListRelationshipTemplatesResponse>> ListTemplates(PaginationFilter? pagination = null)
    {
        return await _client.Get<ListRelationshipTemplatesResponse>($"api/{API_VERSION}/RelationshipTemplates", null, pagination);
    }

    public async Task<ApiResponse<ListRelationshipTemplatesResponse>> ListTemplates(IEnumerable<string> ids, PaginationFilter? pagination = null)
    {
        return await _client
            .Request<ListRelationshipTemplatesResponse>(HttpMethod.Get, $"api/{API_VERSION}/RelationshipTemplates")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("ids", ids)
            .Execute();
    }

    public async Task<ApiResponse<RelationshipTemplate>> GetTemplate(string id)
    {
        return await _client.Get<RelationshipTemplate>($"api/{API_VERSION}/RelationshipTemplates/{id}");
    }

    public async Task<ApiResponse<CreateRelationshipTemplateResponse>> CreateTemplate(CreateRelationshipTemplateRequest request)
    {
        return await _client.Post<CreateRelationshipTemplateResponse>($"api/{API_VERSION}/RelationshipTemplates", request);
    }
}
