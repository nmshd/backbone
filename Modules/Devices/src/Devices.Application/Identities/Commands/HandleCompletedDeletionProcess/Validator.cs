using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.HandleCompletedDeletionProcess;

public class Validator : AbstractValidator<HandleCompletedDeletionProcessCommand>
{
    public Validator()
    {
        RuleFor(c => c.IdentityAddress).ValidId<HandleCompletedDeletionProcessCommand, IdentityAddress>();
        RuleFor(c => c.Usernames).DetailedNotEmpty();
    }
}
