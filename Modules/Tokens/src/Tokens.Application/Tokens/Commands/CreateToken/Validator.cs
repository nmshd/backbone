using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Tooling;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class Validator : AbstractValidator<CreateTokenCommand>
{
    public Validator(IUserContext userContext)
    {
        RuleFor(t => t.ForIdentity)
            .ValidId<CreateTokenCommand, IdentityAddress>()
            .When(t => t.ForIdentity != null);

        RuleFor(c => c.Password).NumberOfBytes(1, Token.MAX_PASSWORD_LENGTH).When(c => c.Password != null);

        RuleFor(t => t.ExpiresAt)
            .GreaterThan(SystemTime.UtcNow).WithMessage("'{PropertyName}' must be in the future.").WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);

        var activeIdentity = userContext.GetAddressOrNull();

        if (activeIdentity == null)
        {
            RuleFor(t => t.Content).DetailedNull();

            RuleFor(t => t.ExpiresAt)
                .LessThan(SystemTime.UtcNow.AddMinutes(5)).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        }
        else
        {
            RuleFor(t => t.Content).NumberOfBytes(0, Token.MAX_CONTENT_LENGTH);
        }
    }
}
