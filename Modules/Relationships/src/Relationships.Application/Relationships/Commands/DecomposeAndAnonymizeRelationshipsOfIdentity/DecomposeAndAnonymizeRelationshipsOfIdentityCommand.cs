using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeAndAnonymizeRelationshipsOfIdentity;

public class DecomposeAndAnonymizeRelationshipsOfIdentityCommand : IRequest
{
    public DecomposeAndAnonymizeRelationshipsOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
