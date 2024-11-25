using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Devices.Domain;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Commands.UpdateActiveDevice;

public class Validator : AbstractValidator<UpdateActiveDeviceCommand>
{
    public Validator()
    {
        RuleFor(c => c.CommunicationLanguage).TwoLetterIsoLanguage().WithErrorCode(DomainErrors.InvalidDeviceCommunicationLanguage().Code);
    }
}
