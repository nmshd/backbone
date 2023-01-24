using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

// ReSharper disable once UnusedMember.Global
public class UpdateDeviceRegistrationValidator : AbstractValidator<UpdateDeviceRegistrationCommand>
{
    public UpdateDeviceRegistrationValidator()
    {
        RuleFor(dto => dto.Platform).In("fcm", "apns");

        RuleFor(dto => dto.Handle)
            .DetailedNotEmpty()
            .Length(10, 500).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
