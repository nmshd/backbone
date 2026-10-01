using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Announcements.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.GetAnnouncementById;

public class Validator : AbstractValidator<GetAnnouncementByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetAnnouncementByIdQuery, AnnouncementId>();
    }
}
