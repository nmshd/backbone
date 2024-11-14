using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AnonymizeRelationshipsOfIdentity;

public class AnonymizeRelationshipsOfIdentityCommand : IRequest
{
    public AnonymizeRelationshipsOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
