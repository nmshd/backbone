using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeRelationship;

public class Validator : AbstractValidator<DecomposeRelationshipCommand>
{
    public Validator()
    {
        RuleFor(x => x.RelationshipId).ValidId<DecomposeRelationshipCommand, RelationshipId>();
    }
}
