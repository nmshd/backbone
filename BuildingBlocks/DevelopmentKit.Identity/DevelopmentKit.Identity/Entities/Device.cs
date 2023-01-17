using Enmeshed.DevelopmentKit.Identity.ValueObjects;

#pragma warning disable CS8618 // turn off nullable checks
namespace Enmeshed.DevelopmentKit.Identity.Entities
{
    public class Device
    {
        public DeviceId Id { get; set; }

        public IdentityAddress IdentityAddress { get; set; }
        public Identity Identity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
        public DeviceId? DeletedByDevice { get; set; }
        public byte[]? DeletionCertificate { get; set; }
    }
}
