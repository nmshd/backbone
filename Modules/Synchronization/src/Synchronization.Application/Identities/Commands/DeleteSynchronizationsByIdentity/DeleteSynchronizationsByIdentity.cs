using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Application.Identities.Commands.DeleteSynchronizationsByIdentity;
public class DeleteSynchronizationsByIdentity(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress)
{
}
