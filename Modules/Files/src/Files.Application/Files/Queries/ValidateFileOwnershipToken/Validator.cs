using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Files.Queries.ValidateFileOwnershipToken;

public class Validator : AbstractValidator<ValidateFileOwnershipTokenQuery>
{
    public Validator()
    {
        RuleFor(f => f.FileId)
            .ValidId<ValidateFileOwnershipTokenQuery, FileId>();
    }
}
