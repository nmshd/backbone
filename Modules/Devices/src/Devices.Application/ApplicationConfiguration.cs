using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application;

public class ApplicationConfiguration
{
    [Required]
    public required PaginationConfiguration Pagination { get; init; }

    [Required]
    [MinLength(3)]
    [MaxLength(45)]
    public required string DidDomainName { get; init; }

    [Required]
    [Range(1, int.MaxValue)]
    public required int MaxNumberOfFeatureFlagsPerIdentity { get; init; }

    public NotificationsConfiguration? Notifications { get; init; }

    public class NotificationsConfiguration
    {
        [Required]
        public List<NotificationTextsConfiguration> Texts { get; init; } = [];
    }

    public class NotificationTextsConfiguration : IValidatableObject
    {
        [Required]
        [MinLength(3)]
        public required string Code { get; init; }

        [Required]
        public required Dictionary<string, NotificationTranslation> Translations { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Translations.ContainsKey(CommunicationLanguage.DEFAULT_LANGUAGE.Value))
                yield return new ValidationResult("The 'en' language is required.", [nameof(Translations)]);
        }
    }

    public class NotificationTranslation
    {
        [Required]
        [MinLength(1)]
        public required string Title { get; init; }

        [Required]
        [MinLength(1)]
        public required string Body { get; init; }
    }
}
