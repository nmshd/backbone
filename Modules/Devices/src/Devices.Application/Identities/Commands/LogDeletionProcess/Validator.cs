using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.LogDeletionProcess;

public class Validator : AbstractValidator<LogDeletionProcessCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<LogDeletionProcessCommand, IdentityAddress>();
    }
}
