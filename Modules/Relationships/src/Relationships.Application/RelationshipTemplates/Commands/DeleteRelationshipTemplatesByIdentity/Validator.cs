using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesByIdentity;
public class Validator : AbstractValidator<DeleteRelationshipTemplatesByIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
