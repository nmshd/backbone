using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Identities.LogDeletionProcess;

public class Validator : AbstractValidator<LogDeletionProcessCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<LogDeletionProcessCommand, IdentityAddress>();
    }
}
