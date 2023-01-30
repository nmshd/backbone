using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.FluentValidation;
using Enmeshed.Tooling;
using Enmeshed.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationshipTemplate;

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
