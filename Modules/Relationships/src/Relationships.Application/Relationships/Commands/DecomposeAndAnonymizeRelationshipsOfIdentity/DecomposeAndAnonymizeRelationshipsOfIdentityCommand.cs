using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeAndAnonymizeRelationshipsOfIdentity;

public class DecomposeAndAnonymizeRelationshipsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
