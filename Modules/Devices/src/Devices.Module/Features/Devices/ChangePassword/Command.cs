using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.ChangePassword;

public class ChangePasswordCommand : IRequest
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}
