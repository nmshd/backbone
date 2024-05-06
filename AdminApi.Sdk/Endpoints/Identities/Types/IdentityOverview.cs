using Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Identities.Types;

public class IdentityOverview
{
    public required string Address { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? CreatedWithClient { get; set; }
    public int? DatawalletVersion { get; set; }
    public required byte IdentityVersion { get; set; }
    public Tier? Tier { get; set; }
    public int? NumberOfDevices { get; set; }
}
