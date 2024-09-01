using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Tokens.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class Validator : AbstractValidator<GetTokenQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetTokenQuery, TokenId>();
    }
}
