using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileContent;

public class Validator : AbstractValidator<GetFileContentQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetFileContentQuery, FileId>();
    }
}
