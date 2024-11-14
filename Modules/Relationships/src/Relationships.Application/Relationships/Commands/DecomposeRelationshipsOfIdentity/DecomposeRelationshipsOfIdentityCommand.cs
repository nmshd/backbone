using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeRelationshipsOfIdentity;

public class DecomposeRelationshipsOfIdentityCommand : IRequest
{
    public DecomposeRelationshipsOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
