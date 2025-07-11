﻿using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
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
        RuleForEach(x => x.Actions).SetValidator(new CreateAnnouncementCommandActionValidator());
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

public class CreateAnnouncementCommandActionValidator : AbstractValidator<CreateAnnouncementCommandAction>
{
    public CreateAnnouncementCommandActionValidator()
    {
        RuleFor(x => x.Link).DetailedNotEmpty().MaximumLength(300);

        RuleFor(x => x.DisplayName)
            .Must(x => x.Any(t => t.Key == AnnouncementLanguage.DEFAULT_LANGUAGE.Value))
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("An action must have a display name for English.");

        RuleForEach(x => x.DisplayName)
            .Must(x => x.Value.Length is <= 30 and > 0)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("A display name must have between 1 and 30 characters.");

        RuleForEach(x => x.DisplayName)
            .Must(x => x.Value.MatchesRegex("^[^\r\n]+\\z"))
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .WithMessage("A display name must not contain line breaks.");
    }
}
