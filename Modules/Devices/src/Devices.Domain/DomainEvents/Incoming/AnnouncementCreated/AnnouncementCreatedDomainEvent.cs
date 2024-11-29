using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Incoming.AnnouncementCreated;

public class AnnouncementCreatedDomainEvent : DomainEvent
{
    public required string Id { get; set; }
    public required string Severity { get; set; }
    public required List<AnnouncementCreatedDomainEventText> Texts { get; set; }
}

public class AnnouncementCreatedDomainEventText
{
    public required string Language { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}
