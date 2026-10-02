using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Tokens.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.GetToken;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<Query, TokenId>();
        RuleFor(x => x.Password).NumberOfBytes(1, Token.MAX_PASSWORD_LENGTH).When(t => t.Password != null);
    }
}
