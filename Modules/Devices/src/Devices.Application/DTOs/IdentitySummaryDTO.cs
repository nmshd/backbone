using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Devices.Application.Devices.DTOs;
using Backbone.Devices.Domain.Entities;

namespace Backbone.Devices.Application.DTOs;

public class IdentitySummaryDTO
{
    public string Address { get; set; }
    public string ClientId { get; set; }
    public byte[] PublicKey { get; set; }

    public string TierId { get; set; }

    public DateTime CreatedAt { get; set; }

    public byte IdentityVersion { get; set; }

    public int NumberOfDevices { get; set; }

    public IEnumerable<DeviceDTO> Devices { get; set; }

    public IdentitySummaryDTO(IdentityAddress address, string clientId, byte[] publicKey, byte identityVersion, DateTime createdAt, IEnumerable<Device> devices, string tierId)
    {
        Address = address.ToString();
        ClientId = clientId;
        PublicKey = publicKey;
        IdentityVersion = identityVersion;
        CreatedAt = createdAt;
        Devices = devices.Select(it => new DeviceDTO()
        {
            CreatedAt = it.CreatedAt,
            CreatedByDevice = it.CreatedByDevice,
            DeletedAt = it.DeletedAt,
            DeletedByDevice = it.DeletedByDevice,
            DeletionCertificate = it.DeletionCertificate,
            Id = it.Id,
            LastLogin = new LastLoginInformation { Time = it.User.LastLoginAt },
            Username = it.User.UserName
        });
        NumberOfDevices = devices.Count();
        TierId = tierId;
    }

}
