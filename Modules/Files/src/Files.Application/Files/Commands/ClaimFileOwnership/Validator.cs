using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Files.Commands.ClaimFileOwnership;

public class Validator : AbstractValidator<ClaimFileOwnershipCommand>
{
    public Validator()
    {
        RuleFor(x => x.FileId).ValidId<ClaimFileOwnershipCommand, FileId>();
        RuleFor(x => x.OwnershipToken)
            .DetailedNotEmpty()
            .Must(x => FileOwnershipToken.IsValid(x!))
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("Invalid ownership token.");
    }
}
