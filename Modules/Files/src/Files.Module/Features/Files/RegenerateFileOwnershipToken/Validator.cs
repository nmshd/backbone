using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Module.Features.Files.RegenerateFileOwnershipToken;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.FileId).ValidId<Command, FileId>();
    }
}
