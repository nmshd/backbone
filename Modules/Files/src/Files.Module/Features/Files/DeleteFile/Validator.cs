using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Module.Features.Files.DeleteFile;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(f => f.Id)
            .ValidId<Command, FileId>();
    }
}
