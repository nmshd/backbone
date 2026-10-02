using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Announcements.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementById;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<Command, AnnouncementId>();
    }
}
