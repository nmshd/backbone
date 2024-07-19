using Backbone.Modules.Files.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileMetadata;

public class Validator : AbstractValidator<GetFileMetadataQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).Must(FileId.IsValid);
    }
}
