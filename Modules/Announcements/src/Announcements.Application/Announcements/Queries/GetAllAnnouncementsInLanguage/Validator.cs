using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncementsInLanguage;

public class Validator : AbstractValidator<GetAllAnnouncementsInLanguageQuery>
{
    public Validator()
    {
        RuleFor(x => x.Language).DetailedNotEmpty().TwoLetterIsoLanguage();
    }
}
