using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.ListAnnouncementsInLanguage;

public class Validator : AbstractValidator<ListAnnouncementsForActiveIdentityInLanguageQuery>
{
    public Validator()
    {
        RuleFor(x => x.Language).DetailedNotEmpty().TwoLetterIsoLanguage();
    }
}
