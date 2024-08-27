using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.FindRelationshipsOfIdentity;

public class FindRelationshipsOfIdentityQuery : IRequest<FindRelationshipsOfIdentityResponse>
{
    public FindRelationshipsOfIdentityQuery(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
