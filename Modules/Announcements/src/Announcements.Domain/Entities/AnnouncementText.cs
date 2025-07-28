using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Announcements.Domain.Entities;

public class AnnouncementText : Entity
{
    // ReSharper disable once UnusedMember.Local
    protected AnnouncementText()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        AnnouncementId = null!;
        Language = null!;
        Title = null!;
        Body = null!;
    }

    public AnnouncementText(AnnouncementLanguage language, string title, string body)
    {
        AnnouncementId = null!; // will be set by EF Core
        Language = language;
        Title = title;
        Body = body;
    }

    public AnnouncementId AnnouncementId { get; }
    public AnnouncementLanguage Language { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
