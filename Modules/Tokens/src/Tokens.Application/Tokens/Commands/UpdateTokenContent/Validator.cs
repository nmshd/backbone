using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Tokens.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.UpdateTokenContent;

public class Validator : AbstractValidator<UpdateTokenContentCommand>
{
    public Validator()
    {
        RuleFor(x => x.TokenId).ValidId<UpdateTokenContentCommand, TokenId>();

        RuleFor(x => x.NewContent).NumberOfBytes(1, Token.MAX_CONTENT_LENGTH);

        RuleFor(c => c.Password).NumberOfBytes(1, Token.MAX_PASSWORD_LENGTH).When(c => c.Password != null);
    }
}
