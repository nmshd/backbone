using Backbone.BuildingBlocks.Application.Identities.Commands;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsByIdentity;
public class DeleteSyncRunsByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress)
{
}
