namespace AdminUi.Infrastructure.DTOs;

public class IdentityOverviewDTO
{
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? CreatedWithClient { get; set; }
    public string? DatawalletVersion { get; set; }
    public byte IdentityVersion { get; set; }
    public string TierName { get; set; }
    public string TierId { get; set; }
    public int? NumberOfDevices { get; set; }
}
