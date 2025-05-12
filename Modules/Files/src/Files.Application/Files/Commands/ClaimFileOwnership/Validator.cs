using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Files.Commands.ClaimFileOwnership;

public class Validator : AbstractValidator<ClaimFileOwnershipCommand>
{
    public Validator()
    {
        RuleFor(x => x.FileId).ValidId<ClaimFileOwnershipCommand, FileId>();
        RuleFor(x => x.OwnershipToken)
            .NotEmpty()
            .WithMessage("Ownership token cannot be empty.");
    }
}
