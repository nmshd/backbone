using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;
public class Validator : AbstractValidator<FindRelationshipsOfIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
