using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Files.Queries.ListFileMetadata;

public class Validator : AbstractValidator<ListFileMetadataQuery>
{
    public Validator()
    {
        RuleForEach(x => x.Ids).ValidId<ListFileMetadataQuery, FileId>();
    }
}
