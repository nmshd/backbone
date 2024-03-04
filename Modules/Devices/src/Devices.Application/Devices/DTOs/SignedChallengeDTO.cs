namespace Backbone.Modules.Devices.Application.Devices.DTOs;

public class SignedChallengeDTO
{
    public required string Challenge { get; set; }
    public required byte[] Signature { get; set; }
}
