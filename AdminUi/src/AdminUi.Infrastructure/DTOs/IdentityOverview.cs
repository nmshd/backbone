namespace Backbone.AdminUi.Infrastructure.DTOs;

public class IdentityOverview
{
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? CreatedWithClient { get; set; }
    public int? DatawalletVersion { get; set; }
    public byte IdentityVersion { get; set; }
    public TierDTO Tier { get; set; }
    public int? NumberOfDevices { get; set; }
}
