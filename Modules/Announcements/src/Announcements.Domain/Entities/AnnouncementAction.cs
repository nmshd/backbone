using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Announcements.Domain.Entities;

public class AnnouncementAction : Entity
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    public AnnouncementAction()
    {
        Id = null!;
        DisplayName = null!;
        Link = null!;
    }

    public AnnouncementAction(Dictionary<AnnouncementLanguage, string> displayName, string link, byte order)
    {
        Id = AnnouncementActionId.New();
        DisplayName = displayName;
        Link = link;
        Order = order;
    }

    public AnnouncementActionId Id { get; }
    public Dictionary<AnnouncementLanguage, string> DisplayName { get; }
    public string Link { get; }
    public byte Order { get; }
}
