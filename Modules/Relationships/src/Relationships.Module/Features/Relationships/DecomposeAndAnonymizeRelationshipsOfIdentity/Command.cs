using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeAndAnonymizeRelationshipsOfIdentity;

public class DecomposeAndAnonymizeRelationshipsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
