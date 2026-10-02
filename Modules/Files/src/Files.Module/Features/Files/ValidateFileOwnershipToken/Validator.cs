using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Module.Features.Files.ValidateFileOwnershipToken;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(f => f.FileId).ValidId<Query, FileId>();
        RuleFor(x => x.OwnershipToken)
            .DetailedNotEmpty()
            .Must(x => FileOwnershipToken.IsValid(x!))
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("Invalid ownership token.");
    }
}
