using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengeByIdentity;

public class DeleteChallengeByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress)
{
}
