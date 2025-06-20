using System.Linq.Expressions;
using Backbone.Modules.Announcements.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database.ValueConverters;

public class AnnouncementActionDisplayNameEntityFrameworkConverter : ValueConverter<Dictionary<AnnouncementLanguage, string>, string>
{
    private const string SEMICOLON_ESCAPE = "###";

    public AnnouncementActionDisplayNameEntityFrameworkConverter() : base(DictionaryToString, StringToDictionary)
    {
    }

    private static Expression<Func<Dictionary<AnnouncementLanguage, string>, string>> DictionaryToString =>
        v => string.Join(";", v.Select(kv => $"{kv.Key.Value}:{kv.Value.Replace(";", SEMICOLON_ESCAPE)}"));

    private static Expression<Func<string, Dictionary<AnnouncementLanguage, string>>> StringToDictionary =>
        v => v
            .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(parts => AnnouncementLanguage.Parse(parts[0]), parts => parts[1].Replace(SEMICOLON_ESCAPE, ";"));
}
