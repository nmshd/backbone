using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Domain;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;

public class Validator : AbstractValidator<CreateAnnouncementCommand>
{
    public Validator()
    {
        RuleFor(x => x.IqlQuery!)
            .Must(AnnouncementIqlQuery.IsValid).When(x => !x.IqlQuery.IsNullOrEmpty())
            .WithErrorCode(DomainErrors.InvalidIqlQueryLengthForAnnouncement().Code)
            .WithMessage(DomainErrors.InvalidIqlQueryLengthForAnnouncement().Message);

        RuleFor(x => x.Texts)
            .Must(x => x.Any(t => t.Language == AnnouncementLanguage.DEFAULT_LANGUAGE.Value))
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("There must be a text for English.");

        RuleFor(x => x.Recipients.Count)
            .LessThanOrEqualTo(100)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);

        RuleForEach(x => x.Texts).SetValidator(new CreateAnnouncementCommandTextValidator());
        RuleForEach(x => x.Recipients).ValidId<CreateAnnouncementCommand, IdentityAddress>();
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
