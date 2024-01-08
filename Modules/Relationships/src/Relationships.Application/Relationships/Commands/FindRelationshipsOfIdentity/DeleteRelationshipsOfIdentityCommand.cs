using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;

public class FindRelationshipsOfIdentityCommand : IRequest<FindRelationshipsOfIdentityResponse>
{
    public FindRelationshipsOfIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
