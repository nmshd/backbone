using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipsOfIdentity;

public class Validator : AbstractValidator<DeleteRelationshipsOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<DeleteRelationshipsOfIdentityCommand, IdentityAddress>();
    }
}
