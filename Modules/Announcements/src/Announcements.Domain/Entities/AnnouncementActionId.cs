using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Announcements.Domain.Entities;

public record AnnouncementActionId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "AAC";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    public AnnouncementActionId(string stringValue) : base(stringValue)
    {
    }

    public static AnnouncementActionId Parse(string stringValue)
    {
        UTILS.Validate(stringValue);

        return new AnnouncementActionId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static AnnouncementActionId New()
    {
        var stringValue = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new AnnouncementActionId(PREFIX + stringValue);
    }
}
