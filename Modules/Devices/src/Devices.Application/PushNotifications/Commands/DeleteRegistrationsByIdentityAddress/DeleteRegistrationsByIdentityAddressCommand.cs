using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteRegistrationsByIdentityAddress;
public class DeleteRegistrationsByIdentityAddressCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress)
{
}
