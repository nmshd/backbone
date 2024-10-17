using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class Validator : AbstractValidator<CreateTokenCommand>
{
    private static readonly int MAX_CONTENT_LENGTH = 10.Mebibytes();

    public Validator()
    {
        RuleFor(t => t.Content)
            .DetailedNotEmpty()
            .NumberOfBytes(1, MAX_CONTENT_LENGTH);

        RuleFor(t => t.ExpiresAt)
            .GreaterThan(SystemTime.UtcNow).WithMessage("'{PropertyName}' must be in the future.").WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);

        RuleFor(t => t.ForIdentity)
            .ValidId<CreateTokenCommand, IdentityAddress>()
            .When(t => t.ForIdentity != null);

        RuleFor(c => c.Password).NumberOfBytes(1, Token.MAX_PASSWORD_LENGTH).When(c => c.Password != null);
    }
}
