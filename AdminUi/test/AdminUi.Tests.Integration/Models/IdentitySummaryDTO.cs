namespace AdminUi.Tests.Integration.Models;

public class IdentitySummaryDTO
{
    public string Address { get; set; }
    public string ClientId { get; set; }
    public string PublicKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public byte IdentityVersion { get; set; }
    public int NumberOfDevices { get; set; }
}
