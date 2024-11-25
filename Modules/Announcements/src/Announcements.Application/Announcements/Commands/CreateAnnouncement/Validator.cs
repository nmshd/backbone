using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;

public class Validator : AbstractValidator<CreateAnnouncementCommand>
{
    public Validator()
    {
        RuleFor(x => x.Texts)
            .Must(x => x.Any(t => t.Language == "en"))
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("There must be a text for English.");

        RuleForEach(x => x.Texts).SetValidator(new CreateAnnouncementCommandTextValidator());
    }
}

public class CreateAnnouncementCommandTextValidator : AbstractValidator<CreateAnnouncementCommandText>
{
    public CreateAnnouncementCommandTextValidator()
    {
        RuleFor(x => x.Language).TwoLetterIsoLanguage();
        RuleFor(x => x.Title).DetailedNotEmpty();
        RuleFor(x => x.Body).DetailedNotEmpty();
    }
}
