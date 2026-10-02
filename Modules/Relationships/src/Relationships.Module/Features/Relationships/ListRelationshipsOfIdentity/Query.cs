using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationshipsOfIdentity;

public class ListRelationshipsOfIdentityQuery : IRequest<ListRelationshipsOfIdentityResponse>
{
    public required string IdentityAddress { get; init; }
}
