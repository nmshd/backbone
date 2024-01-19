using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
