using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Devices.ChangePassword;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.OldPassword).DetailedNotEmpty();
        RuleFor(c => c.NewPassword).DetailedNotEmpty();
    }
}
