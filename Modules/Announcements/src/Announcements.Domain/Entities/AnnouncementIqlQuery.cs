using Backbone.BuildingBlocks.Domain.Exceptions;

namespace Backbone.Modules.Announcements.Domain.Entities;

public record AnnouncementIqlQuery
{
    public const int MIN_LENGTH = 1;
    public const int MAX_LENGTH = 1000;

    private AnnouncementIqlQuery(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static AnnouncementIqlQuery Parse(string value)
    {
        return IsValid(value)
            ? new AnnouncementIqlQuery(value)
            : throw new DomainException(DomainErrors.InvalidIqlQueryLengthForAnnouncement());
    }

    public static bool IsValid(string value)
    {
        return value.Length is >= MIN_LENGTH and <= MAX_LENGTH;
    }

    public override string ToString()
    {
        return Value;
    }
}
