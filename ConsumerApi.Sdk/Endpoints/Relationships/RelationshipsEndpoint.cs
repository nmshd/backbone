﻿using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships;

public class RelationshipsEndpoint(EndpointClient client) : ConsumerApiEndpoint(client)
{
    public async Task<ApiResponse<Relationship>> GetRelationship(string id)
    {
        return await _client.Get<Relationship>($"api/{API_VERSION}/Relationships/{id}");
    }

    public async Task<ApiResponse<ListRelationshipsResponse>> ListRelationships(PaginationFilter? pagination = null)
    {
        return await _client.Get<ListRelationshipsResponse>($"api/{API_VERSION}/Relationships", null, pagination);
    }

    public async Task<ApiResponse<ListRelationshipsResponse>> ListRelationships(IEnumerable<string> ids, PaginationFilter? pagination = null)
    {
        return await _client
            .Request<ListRelationshipsResponse>(HttpMethod.Get, $"api/{API_VERSION}/Relationships")
            .Authenticate()
            .WithPagination(pagination)
            .AddQueryParameter("ids", ids)
            .Execute();
    }

    public async Task<ApiResponse<RelationshipMetadata>> CreateRelationship(CreateRelationshipRequest request)
    {
        return await _client.Post<RelationshipMetadata>($"api/{API_VERSION}/Relationships", request);
    }

    public async Task<ApiResponse<ListRelationshipChangesResponse>> ListChanges(
        PaginationFilter? pagination = null, IEnumerable<string>? ids = null, OptionalDateRange? createdAt = null, OptionalDateRange? completedAt = null,
        OptionalDateRange? modifiedAt = null, bool? onlyPeerChanges = null, string? createdBy = null, string? completedBy = null, string? status = null,
        string? type = null
    )
    {
        var builder = _client
            .Request<ListRelationshipChangesResponse>(HttpMethod.Get, $"api/{API_VERSION}/Relationships/Changes")
            .Authenticate()
            .WithPagination(pagination);

        if (ids != null)
        {
            var arr = ids.ToArray();
            if (arr.Length != 0) builder.AddQueryParameter("ids", arr);
        }

        if (createdAt != null) builder.AddQueryParameter("createdAt", createdAt);
        if (completedAt != null) builder.AddQueryParameter("completedAt", completedAt);
        if (modifiedAt != null) builder.AddQueryParameter("modifiedAt", modifiedAt);
        if (onlyPeerChanges != null) builder.AddQueryParameter("onlyPeerChanges", onlyPeerChanges);
        if (createdBy != null) builder.AddQueryParameter("createdBy", createdBy);
        if (completedBy != null) builder.AddQueryParameter("completedBy", completedBy);
        if (status != null) builder.AddQueryParameter("status", status);
        if (type != null) builder.AddQueryParameter("type", type);

        return await builder.Execute();
    }

    public async Task<ApiResponse<RelationshipChange>> GetChange(string id)
    {
        return await _client.Get<RelationshipChange>($"api/{API_VERSION}/Relationships/Changes/{id}");
    }

    public async Task<ApiResponse<RelationshipMetadata>> AcceptChange(string relationshipId, string changeId, CompleteRelationshipChangeRequest request)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Changes/{changeId}/Accept", request);
    }

    public async Task<ApiResponse<RelationshipMetadata>> RejectChange(string relationshipId, string changeId, CompleteRelationshipChangeRequest request)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Changes/{changeId}/Reject", request);
    }

    public async Task<ApiResponse<RelationshipMetadata>> RevokeChange(string relationshipId, string changeId, CompleteRelationshipChangeRequest request)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Changes/{changeId}/Revoke", request);
    }
}
