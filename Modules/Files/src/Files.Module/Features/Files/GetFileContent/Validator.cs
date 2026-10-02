using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Module.Features.Files.GetFileContent;

public class Validator : AbstractValidator<GetFileContentQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetFileContentQuery, FileId>();
    }
}
