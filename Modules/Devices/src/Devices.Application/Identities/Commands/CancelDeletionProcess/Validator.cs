using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcess;

public class Validator : AbstractValidator<CancelDeletionProcessCommand>
{
    public Validator()
    {
        RuleFor(x => x.DeletionProcessId).ValidId<CancelDeletionProcessCommand, IdentityDeletionProcessId>();
    }
}
