using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities;

public class Identity
{
    public Identity(string? clientId, IdentityAddress address, byte[] publicKey, Tier tier, byte identityVersion)
    {
        ClientId = clientId;
        Address = address;
        PublicKey = publicKey;
        IdentityVersion = identityVersion;
        CreatedAt = SystemTime.UtcNow;
        Devices = new List<Device>();
        Tier = tier;
    }

    public string? ClientId { get; set; }

    public IdentityAddress Address { get; set; }
    public byte[] PublicKey { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<Device> Devices { get; set; }

    public byte IdentityVersion { get; set; }

    public Tier? Tier { get; set; }

    public bool IsNew()
    {
        return Devices.Count < 1;
    }
}
