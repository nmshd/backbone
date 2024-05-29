using System.Globalization;
using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public record CommunicationLanguage
{
    public static readonly CommunicationLanguage DEFAULT_LANGUAGE = new("en");

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
        if (value.Length != LENGTH)
            return DomainErrors.InvalidDeviceCommunicationLanguage($"Device Communication Language length must be {LENGTH}");

        var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
        var culture = cultureInfos.FirstOrDefault(c => c.TwoLetterISOLanguageName == value);

        if (culture == null)
            return DomainErrors.InvalidDeviceCommunicationLanguage("Device Communication Language must be a valid two letter ISO code");

        return null;
    }

    public static implicit operator string(CommunicationLanguage name)
    {
        return name.Value;
    }
}
