using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsByIdentity;

public class FindRelationshipsByIdentityCommand(IdentityAddress identityAddress) : IRequest<FindRelationshipsByIdentityResponse>
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
