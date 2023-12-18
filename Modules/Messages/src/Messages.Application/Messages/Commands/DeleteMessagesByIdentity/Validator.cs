using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Messages.Application.Messages.Commands.DeleteMessagesByIdentity;
public class Validator : AbstractValidator<DeleteMessagesByIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
