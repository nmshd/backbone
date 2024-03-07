using Backbone.ConsumerApi.Sdk.Endpoints.Common;
using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Responses;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships;

public class RelationshipsEndpoint(EndpointClient client) : Endpoint(client)
{
    public async Task<ConsumerApiResponse<Relationship>> GetRelationship(string id) => await _client.Get<Relationship>($"Relationships/{id}");

    public async Task<ConsumerApiResponse<ListRelationshipsResponse>> ListRelationships(PaginationFilter? pagination = null)
        => await _client.Get<ListRelationshipsResponse>("Relationships", null, pagination);

    public async Task<ConsumerApiResponse<ListRelationshipsResponse>> ListRelationships(IEnumerable<string> ids, PaginationFilter? pagination = null) => await _client
        .Request<ListRelationshipsResponse>(HttpMethod.Get, "Relationships")
        .Authenticate()
        .WithPagination(pagination)
        .AddQueryParameter("ids", ids)
        .Execute();

    public async Task<ConsumerApiResponse<RelationshipMetadata>> CreateRelationship(CreateRelationshipRequest request) => await _client.Post<RelationshipMetadata>("Relationships", request);

    public async Task<ConsumerApiResponse<List<RelationshipChange>>> ListChanges(PaginationFilter? pagination = null) =>
        await _client.Get<List<RelationshipChange>>("Relationships/Changes", null, pagination);

    public async Task<ConsumerApiResponse<List<RelationshipChange>>> ListChanges(ListRelationshipChangesParameters parameters)
    {
        var builder = _client
            .Request<List<RelationshipChange>>(HttpMethod.Get, "Relationships/Changes")
            .Authenticate()
            .WithPagination(parameters.Pagination);

        if (parameters.Ids != null && parameters.Ids.Count != 0) builder.AddQueryParameter("ids", parameters.Ids);
        if (parameters.CreatedAt != null) builder.AddQueryParameter("createdAt", parameters.CreatedAt);
        if (parameters.CompletedAt != null) builder.AddQueryParameter("completedAt", parameters.CompletedAt);
        if (parameters.ModifiedAt != null) builder.AddQueryParameter("modifiedAt", parameters.ModifiedAt);
        if (parameters.OnlyPeerChanges != null) builder.AddQueryParameter("onlyPeerChanges", parameters.OnlyPeerChanges);
        if (parameters.CreatedBy != null) builder.AddQueryParameter("createdBy", parameters.CreatedBy);
        if (parameters.CompletedBy != null) builder.AddQueryParameter("completedBy", parameters.CompletedBy);
        if (parameters.Status != null) builder.AddQueryParameter("status", parameters.Status);
        if (parameters.Type != null) builder.AddQueryParameter("type", parameters.Type);

        return await builder.Execute();
    }

    public async Task<ConsumerApiResponse<RelationshipChange>> GetChange(string id) => await _client.Get<RelationshipChange>($"Relationships/Changes/{id}");

    public async Task<ConsumerApiResponse<RelationshipMetadata>> AcceptChange(string relationshipId, string changeId, CompleteRelationshipChangeRequest request)
        => await _client.Put<RelationshipMetadata>($"Relationships/{relationshipId}/Changes/{changeId}/Accept", request);

    public async Task<ConsumerApiResponse<RelationshipMetadata>> RejectChange(string relationshipId, string changeId, CompleteRelationshipChangeRequest request)
        => await _client.Put<RelationshipMetadata>($"Relationships/{relationshipId}/Changes/{changeId}/Reject", request);

    public async Task<ConsumerApiResponse<RelationshipMetadata>> RevokeChange(string relationshipId, string changeId, CompleteRelationshipChangeRequest request)
        => await _client.Put<RelationshipMetadata>($"Relationships/{relationshipId}/Changes/{changeId}/Revoke", request);
}
