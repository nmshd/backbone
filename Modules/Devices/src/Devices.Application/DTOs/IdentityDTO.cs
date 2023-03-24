using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.DTOs;

public class IdentityDTO
{
    public string Address { get; set; }
    public string ClientId { get; set; }
    public byte[] PublicKey { get; set; }

    public DateTime CreatedAt { get; set; }

    public byte IdentityVersion { get; set; }

    public IdentityDTO(IdentityAddress address, string clientId, byte[] publicKey, byte identityVersion, DateTime createdAt)
    {
        Address = address.ToString();
        ClientId = clientId;
        PublicKey = publicKey;
        IdentityVersion = identityVersion;
        CreatedAt = createdAt;
    }

}