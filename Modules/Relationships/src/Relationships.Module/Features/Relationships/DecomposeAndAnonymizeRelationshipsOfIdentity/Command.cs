using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeAndAnonymizeRelationshipsOfIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
