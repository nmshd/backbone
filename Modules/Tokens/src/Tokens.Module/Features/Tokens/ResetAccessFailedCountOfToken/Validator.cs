using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Tokens.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ResetAccessFailedCountOfToken;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.TokenId).ValidId<Command, TokenId>();
    }
}
