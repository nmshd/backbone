using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokensByIdentity;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<ListTokensByIdentityQuery>
{
    public Validator()
    {
        RuleFor(q => q.CreatedBy)
            .Cascade(CascadeMode.Stop)
            .DetailedNotEmpty();

        RuleFor(q => q.PaginationFilter.PageNumber)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);

        RuleFor(q => q.PaginationFilter.PageSize)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
