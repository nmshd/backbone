using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;
public class DeleteIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress)
{
}
