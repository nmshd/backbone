using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Files.Commands.DeleteFile;

public class Validator : AbstractValidator<DeleteFileCommand>
{
    public Validator()
    {
        RuleFor(f => f.Id)
            .ValidId<DeleteFileCommand, FileId>();
    }
}
