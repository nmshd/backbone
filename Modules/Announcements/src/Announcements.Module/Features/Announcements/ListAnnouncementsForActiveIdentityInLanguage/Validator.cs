using Backbone.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsForActiveIdentityInLanguage;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(x => x.Language).DetailedNotEmpty().TwoLetterIsoLanguage();
    }
}
