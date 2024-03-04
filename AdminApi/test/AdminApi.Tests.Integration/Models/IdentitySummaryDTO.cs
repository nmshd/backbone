namespace Backbone.AdminApi.Tests.Integration.Models;

public class IdentitySummaryDTO
{
    public required string Address { get; set; }
    public required string ClientId { get; set; }
    public required string PublicKey { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required byte IdentityVersion { get; set; }
    public required int NumberOfDevices { get; set; }
}
