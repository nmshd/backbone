using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Announcements.Domain.Entities;

public class AnnouncementRecipient : Entity
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    public AnnouncementRecipient()
    {
        AnnouncementId = null!;
        Address = null!;
    }

    public AnnouncementRecipient(IdentityAddress address)
    {
        AnnouncementId = null!; // will be set by EF Core (back navigation property)
        Address = address;
    }

    public AnnouncementId AnnouncementId { get; }
    public IdentityAddress Address { get; }
}
