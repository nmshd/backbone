using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Commands.UpdateActiveDevice;

public class Validator : AbstractValidator<UpdateActiveDeviceCommand>
{
    public Validator()
    {
        RuleFor(c => c.CommunicationLanguage).TwoLetterIsoLanguage();
    }
}
