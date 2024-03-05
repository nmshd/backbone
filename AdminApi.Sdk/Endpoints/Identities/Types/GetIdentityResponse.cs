namespace Backbone.AdminApi.Sdk.Endpoints.Identities.Types;

public class GetIdentityResponse
{
    public required string Address { get; set; }
    public required string? ClientId { get; set; }
    public required byte[] PublicKey { get; set; }
    public required string TierId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required byte IdentityVersion { get; set; }
    public required int NumberOfDevices { get; set; }
    public required List<Device> Devices { get; set; }
    public required List<Quota> Quotas { get; set; }
}

public class Device
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedByDevice { get; set; }
    public required LastLoginInformation LastLogin { get; set; }
}

public class LastLoginInformation
{
    public DateTime? Time { get; set; }
}

public class Quota
{
    public string Id { get; set; }
    public string Source { get; set; }
    public Metric Metric { get; set; }
    public int Max { get; set; }
    public uint Usage { get; set; }
    public string Period { get; set; }
}
