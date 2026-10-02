using MediatR;

namespace Backbone.Modules.Files.Module.Features.Identities.DeleteFilesOfIdentity;

public class DeleteFilesOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
