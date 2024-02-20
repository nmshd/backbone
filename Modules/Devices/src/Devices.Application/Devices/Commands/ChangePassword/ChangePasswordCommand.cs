using MediatR;

namespace Backbone.Modules.Devices.Application.Devices.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}
