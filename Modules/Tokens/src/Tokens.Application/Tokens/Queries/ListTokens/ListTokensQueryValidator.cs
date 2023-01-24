using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using FluentValidation;

namespace Tokens.Application.Tokens.Queries.ListTokens;

// ReSharper disable once UnusedMember.Global
public class ListTokensQueryValidator : AbstractValidator<ListTokensQuery>
{
    public ListTokensQueryValidator()
    {
        RuleFor(t => t.PaginationFilter).SetValidator(new PaginationFilterValidator()).When(t => t != null);
    }
}

public class PaginationFilterValidator : AbstractValidator<PaginationFilter>
{
    public PaginationFilterValidator()
    {
        RuleFor(f => f.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
