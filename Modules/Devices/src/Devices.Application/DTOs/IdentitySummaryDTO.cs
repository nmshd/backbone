using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.DTOs;

public class IdentitySummaryDTO
{
    public IdentitySummaryDTO(Identity identity)
    {
        ClientId = identity.ClientId;

        Address = identity.Address.ToString();
        PublicKey = identity.PublicKey;
        CreatedAt = identity.CreatedAt;

        Devices = identity.Devices.Select(d => new DeviceDTO
        {
            CreatedAt = d.CreatedAt,
            CreatedByDevice = d.CreatedByDevice,
            Id = d.Id,
            LastLogin = new LastLoginInformation { Time = d.User.LastLoginAt },
            Username = d.User.UserName!,
            CommunicationLanguage = d.CommunicationLanguage
        });
        NumberOfDevices = identity.Devices.Count;

        IdentityVersion = identity.IdentityVersion;

        TierId = identity.TierId;
    }

    public string Address { get; set; }
    public string? ClientId { get; set; }
    public byte[] PublicKey { get; set; }
    public DateTime CreatedAt { get; set; }

    public IEnumerable<DeviceDTO> Devices { get; set; }
    public int NumberOfDevices { get; set; }

    public byte IdentityVersion { get; set; }

    public string TierId { get; set; }
}
