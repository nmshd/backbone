using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Messages.Application.Messages.Commands.DeleteMessagesOfIdentity;
public class Validator : AbstractValidator<DeleteMessagesOfIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
