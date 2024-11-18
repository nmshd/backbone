using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeAndAnonymizeRelationshipsOfIdentity;

public class Validator : AbstractValidator<DecomposeAndAnonymizeRelationshipsOfIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<DecomposeAndAnonymizeRelationshipsOfIdentityCommand, IdentityAddress>();
    }
}
