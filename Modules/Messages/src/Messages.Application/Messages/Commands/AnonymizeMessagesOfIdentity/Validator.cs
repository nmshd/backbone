using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Messages.Application.Messages.Commands.AnonymizeMessagesOfIdentity;

public class Validator : AbstractValidator<AnonymizeMessagesOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<AnonymizeMessagesOfIdentityCommand, IdentityAddress>();
    }
}
