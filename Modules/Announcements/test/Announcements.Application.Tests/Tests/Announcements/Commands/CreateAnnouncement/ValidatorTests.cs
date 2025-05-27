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

    private static CreateAnnouncementCommand CreateCommand(List<string>? languages = null, string title = "Test Title", string? body = "Test Body")
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
            }).ToList()
        };

        return command;
    }
}
