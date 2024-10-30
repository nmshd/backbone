using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Announcements.Domain.Entities;

public record AnnouncementId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "ANC";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    public AnnouncementId(string stringValue) : base(stringValue)
    {
    }

    public static AnnouncementId Parse(string stringValue)
    {
        UTILS.Validate(stringValue);

        return new AnnouncementId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static AnnouncementId New()
    {
        var stringValue = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new AnnouncementId(PREFIX + stringValue);
    }
}
