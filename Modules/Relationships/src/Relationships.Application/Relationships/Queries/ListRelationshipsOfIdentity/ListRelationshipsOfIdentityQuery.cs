using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsOfIdentity;

public class ListRelationshipsOfIdentityQuery : IRequest<ListRelationshipsOfIdentityResponse>
{
    public required string IdentityAddress { get; init; }
}
