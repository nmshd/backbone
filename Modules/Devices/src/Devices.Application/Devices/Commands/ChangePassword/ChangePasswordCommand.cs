using MediatR;

namespace Devices.Application.Devices.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}
