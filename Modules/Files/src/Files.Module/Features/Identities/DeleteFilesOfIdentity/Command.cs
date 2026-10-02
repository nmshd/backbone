using MediatR;

namespace Backbone.Modules.Files.Module.Features.Identities.DeleteFilesOfIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
