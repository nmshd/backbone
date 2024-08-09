using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;

public class Validator : AbstractValidator<DeleteFilesOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<DeleteFilesOfIdentityCommand, IdentityAddress>();
    }
}
