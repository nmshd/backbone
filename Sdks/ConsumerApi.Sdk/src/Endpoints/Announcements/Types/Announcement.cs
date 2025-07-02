using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Announcements.Types;

public class Announcement
{
    public required string Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required AnnouncementSeverity Severity { get; set; }

    public required AnnouncementText Text { get; set; }
    public required List<AnnouncementAction> Actions { get; set; }

    public required string? IqlQuery { get; set; }
}

public enum AnnouncementSeverity
{
    Low,
    Medium,
    High
}

public class AnnouncementText
{
    public required string Language { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}

public class AnnouncementAction
{
    public required string DisplayName { get; set; }
    public required string Link { get; set; }
}
