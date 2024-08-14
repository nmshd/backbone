using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<ListTokensQuery>
{
    public Validator()
    {
        RuleFor(t => t.PaginationFilter).SetValidator(new PaginationFilterValidator()).When(t => t != null);
        RuleForEach(x => x.Ids).ValidId<ListTokensQuery, TokenId>();
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
