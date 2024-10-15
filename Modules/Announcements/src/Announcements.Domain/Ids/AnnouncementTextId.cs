using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Announcements.Domain.Ids;
public record AnnouncementTextId(string Value) : StronglyTypedId(Value);
