using Backbone.Modules.Devices.Domain.Entities.Identities;
using DateTime = System.DateTime;

namespace Backbone.Modules.Devices.Application.DTOs;
public class RegisteredIdentityDetailsDTO
{
    public RegisteredIdentityDetailsDTO(Identity identity)
    {
        Address = identity.Address.ToString();
        CreatedAt = identity.CreatedAt;
        IdentityVersion = identity.IdentityVersion;
        TierId = identity.TierId;
        Status = identity.Status;
        DeletionGracePeriodEndsAt = identity.DeletionGracePeriodEndsAt;
    }

    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public byte IdentityVersion { get; set; }
    public string TierId { get; set; }
    public IdentityStatus Status { get; set; }
    public DateTime? DeletionGracePeriodEndsAt { get; set; }
}
