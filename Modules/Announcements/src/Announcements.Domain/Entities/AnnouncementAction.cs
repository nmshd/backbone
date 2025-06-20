using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Announcements.Domain.Entities;

public class AnnouncementAction : Entity
{
    public AnnouncementAction(Dictionary<AnnouncementLanguage, string> displayName, string link)
    {
        Id = AnnouncementActionId.New();
        DisplayName = displayName;
        Link = link;
    }

    public AnnouncementActionId Id { get; }
    public Dictionary<AnnouncementLanguage, string> DisplayName { get; }
    public string Link { get; }
}
