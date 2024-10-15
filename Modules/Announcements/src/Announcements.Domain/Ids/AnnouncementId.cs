using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Announcements.Domain.Ids;
public record AnnouncementId(string Value) : StronglyTypedId(Value)
{
}
