using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Messages.Application.Messages.Commands.DeleteOrphanedMessages;

public class Validator : AbstractValidator<DeleteOrphanedMessagesCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<DeleteOrphanedMessagesCommand, IdentityAddress>();
    }
}
