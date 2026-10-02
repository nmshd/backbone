using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Module.Features.Files.ListFileMetadata;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleForEach(x => x.Ids).ValidId<Query, FileId>();
    }
}
