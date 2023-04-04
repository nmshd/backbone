using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.DTOs;

public class IdentitySummaryDTO
{
    public string Address { get; set; }
    public string ClientId { get; set; }
    public byte[] PublicKey { get; set; }

    public DateTime CreatedAt { get; set; }

    public byte IdentityVersion { get; set; }

    public int NumberOfDevices { get; set; }

    public IdentitySummaryDTO(IdentityAddress address, string clientId, byte[] publicKey, byte identityVersion, DateTime createdAt, int numberOfDevices)
    {
        Address = address.ToString();
        ClientId = clientId;
        PublicKey = publicKey;
        IdentityVersion = identityVersion;
        CreatedAt = createdAt;
        NumberOfDevices = numberOfDevices;
    }

}