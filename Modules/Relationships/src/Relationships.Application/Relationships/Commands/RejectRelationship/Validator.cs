using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationship;

public class Validator : AbstractValidator<RejectRelationshipCommand>
{
    public Validator()
    {
        RuleFor(x => x.RelationshipId).ValidId<RejectRelationshipCommand, RelationshipId>();
    }
}
