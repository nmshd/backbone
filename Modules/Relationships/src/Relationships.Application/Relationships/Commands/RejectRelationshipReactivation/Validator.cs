using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationshipReactivation;

public class Validator : AbstractValidator<RejectRelationshipReactivationCommand>
{
    public Validator()
    {
        RuleFor(x => x.RelationshipId).ValidId<RejectRelationshipReactivationCommand, RelationshipId>();
    }
}

