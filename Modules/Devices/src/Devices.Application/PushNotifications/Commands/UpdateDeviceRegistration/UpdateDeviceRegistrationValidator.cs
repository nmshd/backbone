using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

// ReSharper disable once UnusedMember.Global
public class UpdateDeviceRegistrationValidator : AbstractValidator<UpdateDeviceRegistrationCommand>
{
    public UpdateDeviceRegistrationValidator()
    {
        RuleFor(dto => dto.Platform).In("fcm", "apns");

        RuleFor(dto => dto.Environment).In("Development", "Production").When(dto => dto.Environment != null);

        RuleFor(dto => dto.Handle)
            .DetailedNotEmpty()
            .Length(10, 500).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);

        RuleFor(dto => dto.AppId)
            .DetailedNotEmpty();
    }
}
