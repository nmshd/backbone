using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.ListAllAnnouncementsInLanguage;

public class Validator : AbstractValidator<ListAllAnnouncementsForActiveIdentityInLanguageQuery>
{
    public Validator()
    {
        RuleFor(x => x.Language).DetailedNotEmpty().TwoLetterIsoLanguage();
    }
}
