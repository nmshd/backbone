using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;

public class Validator : AbstractValidator<StartDeletionProcessAsSupportCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<StartDeletionProcessAsSupportCommand, IdentityAddress>();
    }
}
