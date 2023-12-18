using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesByIdentity;
public class Validator : AbstractValidator<DeleteFilesByIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
