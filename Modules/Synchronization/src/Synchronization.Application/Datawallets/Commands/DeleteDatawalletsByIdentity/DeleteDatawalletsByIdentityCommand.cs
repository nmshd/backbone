using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.DeleteDatawalletsByIdentity;
public class DeleteDatawalletsByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress)
{
}
