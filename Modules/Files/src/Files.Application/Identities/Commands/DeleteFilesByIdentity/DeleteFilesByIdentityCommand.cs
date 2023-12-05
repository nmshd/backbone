using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesByIdentity;

public class DeleteFilesByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress)
{
}
