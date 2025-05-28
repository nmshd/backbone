using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsOfIdentity;

public class ListRelationshipsOfIdentityQuery : IRequest<ListRelationshipsOfIdentityResponse>
{
    public ListRelationshipsOfIdentityQuery(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
