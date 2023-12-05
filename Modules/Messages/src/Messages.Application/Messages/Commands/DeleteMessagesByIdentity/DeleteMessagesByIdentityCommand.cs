using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Messages.Application.Messages.Commands.DeleteMessagesByIdentity;

public class DeleteMessagesByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress)
{
}
