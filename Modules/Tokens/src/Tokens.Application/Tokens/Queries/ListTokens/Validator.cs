using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
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

        RuleFor(q => q.QueryItems)
            .Cascade(CascadeMode.Stop)
            .DetailedNotEmpty();

        RuleForEach(x => x.QueryItems)
            .Cascade(CascadeMode.Stop)
            .ChildRules(queryItems =>
            {
                queryItems
                    .RuleFor(query => query.Id)
                    .ValidId<ListTokensQueryItem, TokenId>();
            });
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
