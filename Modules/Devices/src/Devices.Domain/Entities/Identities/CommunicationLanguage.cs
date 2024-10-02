using System.Globalization;
using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public record CommunicationLanguage
{
    public static readonly CommunicationLanguage DEFAULT_LANGUAGE = new("en");
    private static readonly CultureInfo[] CULTURES = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

    public string Value { get; }
    public const int LENGTH = 2;

    private CommunicationLanguage(string value)
    {
        Value = value;
    }

    public static CommunicationLanguage Create(string value)
    {
        if (!IsValid(value))
            throw new InvalidCommunicationLanguageException($"'{value}' is not a valid {nameof(CommunicationLanguage)}.");
        return new CommunicationLanguage(value);
    }

    public static DomainError? Validate(string value)
    {
        if (CULTURES.All(c => c.TwoLetterISOLanguageName != value))
            return DomainErrors.InvalidDeviceCommunicationLanguage();

        return null;
    }

    public static bool IsValid(string value) // todo: Timo: Should we maybe use this instead of Validate method above?
    {
        if (CULTURES.All(c => c.TwoLetterISOLanguageName != value))
            return false;

        return true;
    }

    public static implicit operator string(CommunicationLanguage name)
    {
        return name.Value;
    }
}

public class InvalidCommunicationLanguageException : Exception
{
    public InvalidCommunicationLanguageException(string message) : base(message)
    {
    }
}
