using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Tokens.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteToken;

public class Validator : AbstractValidator<DeleteTokenCommand>
{
    public Validator()
    {
        RuleFor(command => command.Id).ValidId<DeleteTokenCommand, TokenId>();
    }
}
