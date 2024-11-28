using System.Globalization;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.Exceptions;

namespace Backbone.Modules.Announcements.Domain.Entities;

public record AnnouncementLanguage
{
    public static readonly AnnouncementLanguage DEFAULT_LANGUAGE = new("en");
    private static readonly CultureInfo[] CULTURES = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

    public const int LENGTH = 2;

    private AnnouncementLanguage(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static AnnouncementLanguage Parse(string value)
    {
        var validationResult = Validate(value);

        if (validationResult != null)
            throw new DomainException(validationResult);

        return new AnnouncementLanguage(value);
    }

    public static DomainError? Validate(string value)
    {
        if (CULTURES.All(c => c.TwoLetterISOLanguageName != value))
            return DomainErrors.InvalidAnnouncementLanguage();

        return null;
    }
}
