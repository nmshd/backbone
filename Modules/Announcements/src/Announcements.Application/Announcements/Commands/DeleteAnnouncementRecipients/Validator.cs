using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.DeleteAnnouncementRecipients;

public class Validator : AbstractValidator<DeleteAnnouncementRecipientsCommand>
{
    public Validator()
    {
        RuleFor(c => c.IdentityAddress)
            .ValidId<DeleteAnnouncementRecipientsCommand, IdentityAddress>();
    }
}
