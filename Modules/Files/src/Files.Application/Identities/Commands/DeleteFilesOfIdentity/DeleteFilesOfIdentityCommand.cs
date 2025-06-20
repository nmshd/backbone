using MediatR;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;

public class DeleteFilesOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
