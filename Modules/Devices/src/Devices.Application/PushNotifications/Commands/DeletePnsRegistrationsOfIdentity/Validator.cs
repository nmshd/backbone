using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.DeletePnsRegistrationsOfIdentity;

public class Validator : AbstractValidator<DeletePnsRegistrationsOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<DeletePnsRegistrationsOfIdentityCommand, IdentityAddress>();
    }
}
