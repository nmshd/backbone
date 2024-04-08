using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

public class Validator : AbstractValidator<AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand>
{
    public Validator() => RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
}
