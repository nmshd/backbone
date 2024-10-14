using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class Validator : AbstractValidator<CreateRelationshipTemplateCommand>
{
    public Validator()
    {
        RuleFor(c => c.Content).NumberOfBytes(0, 10.Mebibytes());

        RuleFor(c => c.MaxNumberOfAllocations)
            .GreaterThan(0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code).When(c => c.MaxNumberOfAllocations != null);

        RuleFor(c => c.ExpiresAt)
            .GreaterThan(SystemTime.UtcNow).WithMessage("'{PropertyName}' must be in the future.").WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .When(c => c.ExpiresAt != null);

        RuleFor(c => c.ForIdentity)
            .ValidId<CreateRelationshipTemplateCommand, IdentityAddress>()
            .When(c => c.ForIdentity != null);

        RuleFor(c => c.Password).NumberOfBytes(0, RelationshipTemplate.MAX_PASSWORD_LENGTH);
    }
}
