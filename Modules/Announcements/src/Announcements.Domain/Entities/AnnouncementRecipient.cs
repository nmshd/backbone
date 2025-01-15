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
        Address = null!;
    }

    public AnnouncementRecipient(IdentityAddress address)
    {
        AnnouncementId = null!; // will be set by EF Core (back navigation property)
        Address = address;
        CreatedAt = SystemTime.UtcNow;
    }

    public AnnouncementId AnnouncementId { get; }
    public IdentityAddress Address { get; private set; }
    public DateTime CreatedAt { get; }

    public void Anonymize(string didDomainName)
    {
        Address = IdentityAddress.GetAnonymized(didDomainName);
    }
}
