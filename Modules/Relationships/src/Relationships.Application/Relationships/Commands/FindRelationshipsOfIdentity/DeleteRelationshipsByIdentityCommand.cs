using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;

public class FindRelationshipsOfIdentityCommand(IdentityAddress identityAddress) : IRequest<FindRelationshipsByIdentityResponse>
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
