using Backbone.BuildingBlocks.SDK.Endpoints.Common;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships;

public class RelationshipsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ApiResponse<Relationship>> GetRelationship(string id) => await _client.Get<Relationship>($"Relationships/{id}");

    public async Task<ApiResponse<ListRelationshipsResponse>> ListRelationships(PaginationFilter? pagination = null)
        => await _client.Get<ListRelationshipsResponse>("Relationships", null, pagination);

    public async Task<ApiResponse<ListRelationshipsResponse>> ListRelationships(IEnumerable<string> ids, PaginationFilter? pagination = null) => await _client
        .Request<ListRelationshipsResponse>(HttpMethod.Get, "Relationships")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("ids", ids)
        .Execute();

    public async Task<ApiResponse<RelationshipMetadata>> CreateRelationship(CreateRelationshipRequest request)
        => await _client.Post<RelationshipMetadata>("Relationships", request);

    public async Task<ApiResponse<ListRelationshipChangesResponse>> ListChanges(
        PaginationFilter? pagination = null, IEnumerable<string>? ids = null, OptionalDateRange? createdAt = null, OptionalDateRange? completedAt = null,
        OptionalDateRange? modifiedAt = null, bool? onlyPeerChanges = null, string? createdBy = null, string? completedBy = null, string? status = null,
        string? type = null
        )
    {
        var builder = _client
            .Request<ListRelationshipChangesResponse>(HttpMethod.Get, "Relationships/Changes")
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

    public async Task<ApiResponse<RelationshipChange>> GetChange(string id) => await _client.Get<RelationshipChange>($"Relationships/Changes/{id}");

    public async Task<ApiResponse<RelationshipMetadata>> AcceptChange(string relationshipId, string changeId, CompleteRelationshipChangeRequest request)
        => await _client.Put<RelationshipMetadata>($"Relationships/{relationshipId}/Changes/{changeId}/Accept", request);

    public async Task<ApiResponse<RelationshipMetadata>> RejectChange(string relationshipId, string changeId, CompleteRelationshipChangeRequest request)
        => await _client.Put<RelationshipMetadata>($"Relationships/{relationshipId}/Changes/{changeId}/Reject", request);

    public async Task<ApiResponse<RelationshipMetadata>> RevokeChange(string relationshipId, string changeId, CompleteRelationshipChangeRequest request)
        => await _client.Put<RelationshipMetadata>($"Relationships/{relationshipId}/Changes/{changeId}/Revoke", request);
}
