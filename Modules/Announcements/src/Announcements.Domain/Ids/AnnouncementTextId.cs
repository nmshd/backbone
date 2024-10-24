using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Announcements.Domain.Ids;

public record AnnouncementTextId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "ANT";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    public AnnouncementTextId(string stringValue) : base(stringValue)
    {

    }

    public static AnnouncementTextId Parse(string stringValue)
    {
        UTILS.Validate(stringValue);

        return new AnnouncementTextId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static AnnouncementTextId New()
    {
        var stringValue = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new AnnouncementTextId(PREFIX + stringValue);
    }
}
