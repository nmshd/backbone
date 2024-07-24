using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;

public class Validator : AbstractValidator<CancelDeletionAsSupportCommand>
{
    public Validator()
    {
        RuleFor(x => x.Address).ValidId<CancelDeletionAsSupportCommand, IdentityAddress>();
        RuleFor(x => x.DeletionProcessId).ValidId<CancelDeletionAsSupportCommand, IdentityDeletionProcessId>();
    }
}
