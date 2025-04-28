using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Files.Commands.CreateFile;

public class Validator : AbstractValidator<CreateFileCommand>
{
    public Validator()
    {
        RuleFor(r => r.FileContent)
            .DetailedNotNull()
            .NumberOfBytes(1, 15.Mebibytes());

        RuleFor(r => r.CipherHash)
            .DetailedNotNull()
            .NumberOfBytes(1, 128); // SHA-1024

        RuleFor(r => r.ExpiresAt)
            .DetailedNotEmpty()
            .GreaterThan(SystemTime.UtcNow).WithMessage("'{PropertyName}' must be in the future.").WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);

        RuleFor(m => m.OwnerSignature)
            .NumberOfBytes(1, 512);

        RuleFor(r => r.EncryptedProperties)
            .NumberOfBytes(0, 1.Mebibytes());
    }
}
