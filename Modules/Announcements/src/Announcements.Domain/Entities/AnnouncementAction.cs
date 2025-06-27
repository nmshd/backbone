using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Announcements.Domain.Entities;

public class AnnouncementAction : Entity
{
    // ReSharper disable once UnusedMember.Local
    public AnnouncementAction()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        DisplayName = null!;
        Link = null!;
    }

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
