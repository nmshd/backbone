using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeleteRegistrationsOfIdentity;
public class Validator : AbstractValidator<DeleteRegistrationsOfIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
