using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Devices.Commands.ChangePassword;

// ReSharper disable once UnusedMember.Global
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.OldPassword).DetailedNotEmpty();
        RuleFor(c => c.NewPassword).DetailedNotEmpty();
    }
}
