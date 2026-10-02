using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Files.Module.Features.Identities.DeleteFilesOfIdentity;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<Command, IdentityAddress>();
    }
}
