using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsInLanguage;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(x => x.Language).DetailedNotEmpty().TwoLetterIsoLanguage();
    }
}
