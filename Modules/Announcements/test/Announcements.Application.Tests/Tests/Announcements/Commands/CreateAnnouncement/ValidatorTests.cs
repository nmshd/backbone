using Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;
using Backbone.Modules.Announcements.Domain.Entities;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Announcements.Application.Tests.Tests.Announcements.Commands.CreateAnnouncement;

public class ValidatorTests
{
    [Fact]
    public void Accepts_a_valid_object()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(CreateCommand());

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Expects_an_english_text()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(CreateCommand(languages: ["de"]));

        // Assert
        validationResult.ShouldHaveValidationErrorFor(c => c.Texts).WithErrorMessage("There must be a text for English.");
    }

    [Fact]
    public void Expects_two_letter_language_codes()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(CreateCommand(languages: ["eng", "e"]));

        // Assert
        validationResult.ShouldHaveValidationErrorFor("Texts[0].Language").WithErrorCode("error.platform.validation.invalidPropertyValue")
            .WithErrorMessage("This language is not a valid two letter ISO language name.");
        validationResult.ShouldHaveValidationErrorFor("Texts[1].Language").WithErrorCode("error.platform.validation.invalidPropertyValue")
            .WithErrorMessage("This language is not a valid two letter ISO language name.");
    }

    [Fact]
    public void Expects_non_empty_title_and_body()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(CreateCommand(title: "", body: null));

        // Assert
        validationResult.ShouldHaveValidationErrorFor("Texts[0].Title").WithErrorCode("error.platform.validation.invalidPropertyValue");
        validationResult.ShouldHaveValidationErrorFor("Texts[0].Body").WithErrorCode("error.platform.validation.invalidPropertyValue");
    }

    [Fact]
    public void Expects_an_english_display_name_for_actions()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(CreateCommand(action: new CreateAnnouncementCommandAction
        {
            Link = "https://enmeshed.eu", DisplayName = new Dictionary<string, string>
            {
                { "de", "Test Titel" }
            }
        }));

        // Assert
        validationResult.ShouldHaveValidationErrorFor("Actions[0].DisplayName").WithErrorMessage("An action must have a display name for English.");
    }

    private static CreateAnnouncementCommand CreateCommand(List<string>? languages = null, string title = "Test Title", string? body = "Test Body", CreateAnnouncementCommandAction? action = null)
    {
        languages ??= ["en", "de"];

        var command = new CreateAnnouncementCommand
        {
            ExpiresAt = null,
            Severity = AnnouncementSeverity.Low,
            IsSilent = false,
            Texts = languages.Select(l => new CreateAnnouncementCommandText
            {
                Language = l,
                Title = title,
                Body = body!
            }).ToList(),
            Actions = action == null ? [] : [action]
        };

        return command;
    }
}
