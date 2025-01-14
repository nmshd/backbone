using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.AnonymizeRecipient;

public class Validator : AbstractValidator<AnonymizeRecipientForIdentityCommand>
{
    public Validator()
    {
        RuleFor(c => c.IdentityAddress)
            .ValidId<AnonymizeRecipientForIdentityCommand, IdentityAddress>();
    }
}
