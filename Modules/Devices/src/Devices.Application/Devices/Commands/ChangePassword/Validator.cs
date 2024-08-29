using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Commands.ChangePassword;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<ChangePasswordCommand>
{
    public Validator()
    {
        RuleFor(c => c.OldPassword).DetailedNotEmpty();
        RuleFor(c => c.NewPassword).DetailedNotEmpty();
    }
}
