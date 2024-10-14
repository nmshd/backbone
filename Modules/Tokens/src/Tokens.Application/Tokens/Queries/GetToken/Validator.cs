using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Tokens.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class Validator : AbstractValidator<GetTokenQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetTokenQuery, TokenId>();
        RuleFor(x => x.Password).NumberOfBytes(1, Token.MAX_PASSWORD_LENGTH).When(x => x.Password != null);
    }
}
