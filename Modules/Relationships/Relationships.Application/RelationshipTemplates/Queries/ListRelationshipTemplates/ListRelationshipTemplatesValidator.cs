using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

// ReSharper disable once UnusedMember.Global
public class ListRelationshipTemplatesValidator : AbstractValidator<ListRelationshipTemplatesQuery>
{
    public ListRelationshipTemplatesValidator()
    {
        RuleFor(q => q.Ids)
            .Cascade(CascadeMode.Stop)
            .DetailedNotNull()
            .Must(ids => ids.Count > 0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code).WithMessage("'Ids' must not be empty.");
    }
}
