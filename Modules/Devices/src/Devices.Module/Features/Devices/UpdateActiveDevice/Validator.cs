using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Devices.Domain;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Devices.UpdateActiveDevice;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.CommunicationLanguage).TwoLetterIsoLanguage().WithErrorCode(DomainErrors.InvalidDeviceCommunicationLanguage().Code);
    }
}
