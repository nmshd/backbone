namespace Backbone.AdminApi.DTOs;

public class IdentityOverviewDTO
{
    public required string Address { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? CreatedWithClient { get; set; }
    public int? DatawalletVersion { get; set; }
    public required byte IdentityVersion { get; set; }
    public required TierDTO Tier { get; set; }
    public int? NumberOfDevices { get; set; }
}
