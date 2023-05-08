namespace AdminApi.Tests.Integration.Models;

public class IdentitySummaryDTO
{
    public string Address { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public byte IdentityVersion { get; set; }
    public int NumberOfDevices { get; set; }
}
