using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Notifications.Commands.SendNotification;

public class Validator : AbstractValidator<SendNotificationCommand>
{
    public Validator()
    {
        RuleFor(x => x.Code).DetailedNotEmpty();
        RuleFor(x => x.Recipients.Length).InclusiveBetween(1, 100).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        RuleForEach(x => x.Recipients).ValidId<SendNotificationCommand, IdentityAddress>();
    }
}
