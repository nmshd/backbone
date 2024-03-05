using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class CreateRelationshipTemplateCommandValidator : AbstractValidator<CreateRelationshipTemplateCommand>
{
    public CreateRelationshipTemplateCommandValidator()
    {
        RuleFor(c => c.Content).NumberOfBytes(0, 10.Mebibytes());

        RuleFor(c => c.MaxNumberOfAllocations)
            .GreaterThan(0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code).When(c => c.MaxNumberOfAllocations != null);

        RuleFor(c => c.ExpiresAt)
            .GreaterThan(SystemTime.UtcNow).WithMessage("'{PropertyName}' must be in the future.").WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code).When(c => c.ExpiresAt != null);
    }
}
