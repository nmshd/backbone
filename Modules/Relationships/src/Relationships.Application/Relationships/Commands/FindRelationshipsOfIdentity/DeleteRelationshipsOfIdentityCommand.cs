using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;

public class FindRelationshipsOfIdentityQuery : IRequest<FindRelationshipsOfIdentityResponse>
{
    public FindRelationshipsOfIdentityQuery(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
