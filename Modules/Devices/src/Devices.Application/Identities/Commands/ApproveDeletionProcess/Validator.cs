using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;

public class Validator : AbstractValidator<ApproveDeletionProcessCommand>
{
    public Validator()
    {
        RuleFor(x => x.DeletionProcessId).ValidId<ApproveDeletionProcessCommand, IdentityDeletionProcessId>();
    }
}
