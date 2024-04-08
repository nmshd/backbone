using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;

// ReSharper disable once UnusedMember.Global
public class ListRelationshipsValidator : AbstractValidator<ListRelationshipsQuery>
{
    public ListRelationshipsValidator()
    {
        RuleFor(q => q.Ids)
            .Cascade(CascadeMode.Stop)
            .DetailedNotNull()
            .Must(ids => ids.Count > 0).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code).WithMessage("'Ids' must not be empty.");
    }
}
