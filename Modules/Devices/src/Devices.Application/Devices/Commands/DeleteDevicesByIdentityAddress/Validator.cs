using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevicesByIdentityAddress;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<DeleteDevicesByIdentityAddressCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
    }
}
