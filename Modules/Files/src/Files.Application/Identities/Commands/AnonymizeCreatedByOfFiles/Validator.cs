using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Identities.Commands.AnonymizeCreatedByOfFiles;

public class Validator : AbstractValidator<AnonymizeCreatedByOfFilesCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<AnonymizeCreatedByOfFilesCommand, IdentityAddress>();
    }
}
