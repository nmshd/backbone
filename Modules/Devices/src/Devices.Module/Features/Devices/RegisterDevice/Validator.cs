using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Devices.Shared.Validators;
using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Devices.RegisterDevice;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.DevicePassword).DetailedNotEmpty();
        RuleFor(c => c.SignedChallenge).DetailedNotEmpty().SetValidator(new SignedChallengeDTOValidator());
        RuleFor(c => c.CommunicationLanguage).Valid(CommunicationLanguage.Validate);
    }
}
