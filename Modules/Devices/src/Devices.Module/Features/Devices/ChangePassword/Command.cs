using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.ChangePassword;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ChangePasswordCommand")]
public class Command : IRequest
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}
