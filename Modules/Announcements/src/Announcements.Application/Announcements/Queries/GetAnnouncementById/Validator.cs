using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Announcements.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAnnouncementById;

public class Validator : AbstractValidator<GetAnnouncementByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetAnnouncementByIdQuery, AnnouncementId>();
    }
}
