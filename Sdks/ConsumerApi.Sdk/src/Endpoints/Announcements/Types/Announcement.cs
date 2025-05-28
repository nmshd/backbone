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

    public required string? IqlQuery { get; set; }
}

public class AnnouncementText
{
    public required string Language { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}

public enum AnnouncementSeverity
{
    Low,
    Medium,
    High
}
