using System.Globalization;
using System.Text.Json;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;
using FluentValidation.Validators;

namespace Backbone.BuildingBlocks.Application.FluentValidation;

public class TwoLetterIsoLanguageValidator<T> : IPropertyValidator<T, string>
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly string[] VALID_LANGUAGES = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures).Select(c => c.TwoLetterISOLanguageName).Distinct().ToArray();

    public bool IsValid(ValidationContext<T> context, string value)
    {
        Console.WriteLine("Number of valid languages: " + JsonSerializer.Serialize(VALID_LANGUAGES.Length));
        Console.WriteLine("Valid languages: " + JsonSerializer.Serialize(VALID_LANGUAGES));
        return VALID_LANGUAGES.Contains(value);
    }

    public string GetDefaultMessageTemplate(string errorCode)
    {
        return "This language is not a valid two letter ISO language name.";
    }

    public string Name { get; } = GenericApplicationErrors.Validation.InvalidPropertyValue().Code;
}

public static class TwoLetterIsoLanguageValidatorRuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> TwoLetterIsoLanguage<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new TwoLetterIsoLanguageValidator<T>());
    }
}
