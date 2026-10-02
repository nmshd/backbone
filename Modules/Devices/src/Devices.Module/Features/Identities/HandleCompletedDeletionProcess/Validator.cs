using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Identities.HandleCompletedDeletionProcess;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.IdentityAddress).ValidId<Command, IdentityAddress>();
        RuleFor(c => c.Usernames).DetailedNotEmpty();
    }
}
