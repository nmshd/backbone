using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Announcements.Domain.Entities;
using FluentValidation;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementById;

public class Validator : AbstractValidator<DeleteAnnouncementByIdCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<DeleteAnnouncementByIdCommand, AnnouncementId>();
    }
}
