using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Tokens.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.DeleteToken;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(command => command.Id).ValidId<Command, TokenId>();
    }
}
