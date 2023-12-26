using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.DeleteIdentity;
public class Validator : AbstractValidator<DeleteIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(IdentityAddress.IsValid);
}
