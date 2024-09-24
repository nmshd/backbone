using System.Globalization;
using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

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

    public static Result<CommunicationLanguage, DomainError> Create(string value)
    {
        var validationResult = Validate(value);
        if (validationResult != null)
            return Result.Failure<CommunicationLanguage, DomainError>(validationResult);
        return Result.Success<CommunicationLanguage, DomainError>(new CommunicationLanguage(value));
    }

    public static DomainError? Validate(string value)
    {
        if (CULTURES.All(c => c.TwoLetterISOLanguageName != value))
            return DomainErrors.InvalidDeviceCommunicationLanguage();

        return null;
    }

    public static bool IsValid(string value)
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
