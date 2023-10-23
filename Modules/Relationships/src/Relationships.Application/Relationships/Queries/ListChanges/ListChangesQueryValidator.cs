using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Relationships.Common;
using Backbone.Relationships.Common.FluentValidation;
using FluentValidation;

namespace Backbone.Relationships.Application.Relationships.Queries.ListChanges;

// ReSharper disable once UnusedMember.Global
public class ListChangesQueryValidator : AbstractValidator<ListChangesQuery>
{
    public ListChangesQueryValidator()
    {
        RuleFor(query => query.CreatedAt)
            .IsValidRange<ListChangesQuery, OptionalDateRange, DateTime?>().WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
