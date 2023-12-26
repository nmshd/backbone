using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevicesOfIdentity;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<DeleteDevicesOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).Must(x => IdentityAddress.IsValid(x));
    }
}
