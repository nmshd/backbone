using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Announcements.Domain.Entities;

public class AnnouncementRecipient : Entity
{
    // ReSharper disable once UnusedMember.Local
    public AnnouncementRecipient()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        AnnouncementId = null!;
        DeviceId = null!;
        Address = null!;
    }

    public AnnouncementRecipient(string deviceId, string address)
    {
        AnnouncementId = null!; // will be set by EF Core (back navigation property)
        DeviceId = deviceId;
        Address = address;
        CreatedAt = SystemTime.UtcNow;
    }

    public AnnouncementId AnnouncementId { get; set; }
    public string DeviceId { get; set; }
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }

    public void Anonymize(string didDomainName)
    {
        DeviceId = "Anonymized";
        Address = IdentityAddress.GetAnonymized(didDomainName);
    }
}
