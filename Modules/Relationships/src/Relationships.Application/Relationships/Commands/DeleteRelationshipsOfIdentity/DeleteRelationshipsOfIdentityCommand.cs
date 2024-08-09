using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipsOfIdentity;

public class DeleteRelationshipsOfIdentityCommand : IRequest
{
    public DeleteRelationshipsOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
