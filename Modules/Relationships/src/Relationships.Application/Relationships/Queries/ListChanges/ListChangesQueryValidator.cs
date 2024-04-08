using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Relationships.Common;
using Backbone.Modules.Relationships.Common.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListChanges;

// ReSharper disable once UnusedMember.Global
public class ListChangesQueryValidator : AbstractValidator<ListChangesQuery>
{
    public ListChangesQueryValidator()
    {
        RuleFor(query => query.CreatedAt)
            .IsValidRange<ListChangesQuery, OptionalDateRange?, DateTime?>().WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
