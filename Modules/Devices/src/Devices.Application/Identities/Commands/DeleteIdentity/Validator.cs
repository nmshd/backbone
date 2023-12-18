using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.CreateAuditLog;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.DeleteIdentity;
public class Validator : AbstractValidator<DeleteIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
