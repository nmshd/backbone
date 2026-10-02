namespace Backbone.Modules.Devices.Module.Features.Devices.Shared;

public class SignedChallengeDTO
{
    public required string Challenge { get; set; }
    public required byte[] Signature { get; set; }
}
