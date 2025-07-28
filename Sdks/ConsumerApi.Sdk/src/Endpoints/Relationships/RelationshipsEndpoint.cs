using Backbone.BuildingBlocks.SDK.Endpoints.Common;
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

    public async Task<ApiResponse<RelationshipMetadata>> AcceptRelationship(string relationshipId, AcceptRelationshipRequest request)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Accept", request);
    }

    public async Task<ApiResponse<RelationshipMetadata>> RejectRelationship(string relationshipId, RejectRelationshipRequest request)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Reject", request);
    }

    public async Task<ApiResponse<RelationshipMetadata>> RevokeRelationship(string relationshipId, RevokeRelationshipRequest request)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Revoke", request);
    }

    public async Task<ApiResponse<RelationshipMetadata>> RevokeRelationshipReactivation(string relationshipId)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Reactivate/Revoke");
    }

    public async Task<ApiResponse<RelationshipMetadata>> TerminateRelationship(string relationshipId)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Terminate");
    }

    public async Task<ApiResponse<RelationshipMetadata>> ReactivateRelationship(string relationshipId)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Reactivate");
    }

    public async Task<ApiResponse<RelationshipMetadata>> AcceptReactivationOfRelationship(string relationshipId)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Reactivate/Accept");
    }

    public async Task<ApiResponse<RelationshipMetadata>> RejectReactivationOfRelationship(string relationshipId)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Reactivate/Reject");
    }

    public async Task<ApiResponse<RelationshipMetadata>> DecomposeRelationship(string relationshipId)
    {
        return await _client.Put<RelationshipMetadata>($"api/{API_VERSION}/Relationships/{relationshipId}/Decompose");
    }

    public async Task<ApiResponse<CanEstablishRelationshipResponse>> CanCreateRelationship(string peerIdentityAddress)
    {
        return await _client
            .Request<CanEstablishRelationshipResponse>(HttpMethod.Get, $"api/{API_VERSION}/Relationships/CanCreate")
            .Authenticate()
            .AddQueryParameter("peer", peerIdentityAddress)
            .Execute();
    }
}
