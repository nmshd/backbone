using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AnonymizeRelationshipsOfIdentity;

public class Validator : AbstractValidator<AnonymizeRelationshipsOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<AnonymizeRelationshipsOfIdentityCommand, IdentityAddress>();
    }
}
