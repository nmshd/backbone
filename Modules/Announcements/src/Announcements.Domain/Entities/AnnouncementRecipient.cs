using Backbone.BuildingBlocks.Domain;
using Backbone.Tooling;

namespace Backbone.Modules.Announcements.Domain.Entities;

public class AnnouncementRecipient : Entity
{
    // ReSharper disable once UnusedMember.Local
    public AnnouncementRecipient()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        AnnouncementId = null!;
        Address = null!;
    }

    public AnnouncementRecipient(string address)
    {
        Id = null!; // will be set by EF Core (primary key)
        AnnouncementId = null!; // will be set by EF Core (back navigation property)
        Address = address;
        CreatedAt = SystemTime.UtcNow;
    }

    public string Id { get; set; }
    public AnnouncementId AnnouncementId { get; }
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
}
