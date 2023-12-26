using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;

public class DeleteFilesOfIdentityCommand(IdentityAddress identityAddress) : IRequest
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
