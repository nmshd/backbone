using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class CreateTokenCommandValidator : AbstractValidator<CreateTokenCommand>
{
    private static readonly int MAX_CONTENT_LENGTH = 10.Mebibytes();

    public CreateTokenCommandValidator()
    {
        RuleFor(t => t.Content)
            .DetailedNotEmpty()
            .NumberOfBytes(1, MAX_CONTENT_LENGTH);

        RuleFor(t => t.ExpiresAt)
            .GreaterThan(SystemTime.UtcNow).WithMessage("'{PropertyName}' must be in the future.").WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
