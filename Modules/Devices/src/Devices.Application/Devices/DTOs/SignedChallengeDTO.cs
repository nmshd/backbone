namespace Backbone.Modules.Devices.Application.Devices.DTOs;

public class SignedChallengeDTO
{
    public string? Challenge { get; set; }
    public byte[]? Signature { get; set; }
}
